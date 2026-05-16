using Financier.Common.Attribute;
using Financier.DataAccess.Abstractions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Financier.Reports
{
    [Header("Saldo")]
    public class ReportStructureSaldoVM : BaseReportVM<ReportStructureSaldoModel>
    {
        public static ObservableCollection<string> Ranges { get; } = new ObservableCollection<string>
        {
"Current Year",
"Last 6 Months",
"Last 12 Months" ,
"Last 2 Years" ,
"Last 24 Months",
        };

        private string _range;
        public string Range
        {
            get => _range;
            set
            {
                if (SetProperty(ref _range, value))
                {
                    RaisePropertyChanged(nameof(Range));
                }
            }
        }

        private bool _isUsdCurrencySelected;
        public bool IsUsdCurrencySelected
        {
            get => _isUsdCurrencySelected;
            set
            {
                if (SetProperty(ref _isUsdCurrencySelected, value))
                {
                    RaisePropertyChanged(nameof(IsUsdCurrencySelected));
                }
            }
        }

        private const string BaseSqlText = @" /* ReportStructureSaldoVM */
SELECT account_title,
       account_id,
       account_is_active,
       is_include_into_totals,
       account_type,
       sort_order,
       balance,
       symbol,
       balance_default_crr,
       balance_usd,
       default_crr_symbol,
       date
FROM   (SELECT a.title AS account_title,
               a.is_active AS account_is_active,
               a.is_include_into_totals,
               a.sort_order,
               a.type as account_type,
               a._id AS account_id,
               Row_number() OVER ( partition BY a._id
                                   ORDER BY Date(t.datetime / 1000, 'unixepoch') DESC, t.datetime DESC
               ) AS RowNum,
               r.balance / 100.0 AS balance,
               c.symbol,
               CASE( SELECT _id FROM currency WHERE is_default = 1)
               WHEN c._id THEN r.balance / 100.0
               ELSE Round((r.balance / 100.0 ) * (SELECT rate
                                                  FROM v_currency_exchange_rate
                                                  WHERE to_currency_id = (SELECT _id FROM currency WHERE is_default = 1)
                                                        AND from_currency_id = c._id
                                                        AND(({0} BETWEEN rate_date AND rate_date_end) OR rate_date_end = 253402293599000 )), 0)
               END AS balance_default_crr,
               CASE( SELECT _id FROM currency WHERE name = 'USD')
               WHEN c._id THEN r.balance / 100.0
               ELSE Round((r.balance / 100.0 ) * (SELECT rate
                                                  FROM v_currency_exchange_rate
                                                  WHERE to_currency_id = (SELECT _id FROM currency WHERE name = 'USD')
                                                        AND from_currency_id = c._id
                                                        AND(({0} BETWEEN rate_date AND rate_date_end) OR rate_date_end = 253402293599000 )), 0)
               END AS balance_usd,
               (SELECT symbol FROM   currency WHERE  is_default = 1) AS default_crr_symbol,
               Date(t.datetime / 1000, 'unixepoch') AS date
        FROM running_balance r
             INNER JOIN account a ON a._id = r.account_id
             INNER JOIN currency c ON a.currency_id = c._id
             INNER JOIN transactions t ON t._id = r.transaction_id
        WHERE t.datetime <= {0}
        ORDER BY a._id, r.datetime DESC ) rep
WHERE RowNum = 1
ORDER BY account_is_active DESC, sort_order ASC

";

        public ReportStructureSaldoVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
            Range = "Last 6 Months";
            IsUsdCurrencySelected = true;
        }

        protected override async Task RefreshData()
        {
            var currentList = new List<ReportStructureSaldoModel>();
            var availableDates = GetDatesRange();
            foreach (var date in availableDates)
            {
                var unixDate = new DateTimeOffset(date.ToDateTime(new TimeOnly(00, 00, 00))).ToUnixTimeMilliseconds();
                var filter = $" {unixDate}";
                var sql = string.Format(BaseSqlText, filter);
                var data = await base.db.ExecuteQuery<ReportStructureSaldoRawModel>(sql);

                if (data != null && data.Any())
                {
                    double? assetsDefaultCurrencyBalance = 0;
                    double? liabilitiesDefaultCurrencyBalance = 0;
                    double? assetsUSDBalance = 0;
                    double? liabilitiesUSDBalance = 0;

                    foreach (var item in data.Where(x => x.AccountIsIncludeInTotals))
                    {
                        if (item.AccountType != "LIABILITY")
                        {
                            assetsDefaultCurrencyBalance += item.DefaultCurrencyBalance;
                            assetsUSDBalance += item.USDBalance;
                        }
                        else
                        {
                            liabilitiesDefaultCurrencyBalance += item.DefaultCurrencyBalance;
                            liabilitiesUSDBalance += item.USDBalance;
                        }
                    }
                    currentList.Add(new ReportStructureSaldoModel
                    {
                        Date = date,
                        DefaultCurrencySymbol = data.FirstOrDefault()?.DefaultCurrencySymbol,
                        AssetsUSDBalance = Math.Round(assetsUSDBalance ?? 0, 2),
                        AssetsDefaultCurrencyBalance = Math.Round(assetsDefaultCurrencyBalance ?? 0, 2),
                        LiabilitiesUSDBalance = Math.Round(liabilitiesUSDBalance ?? 0, 2),
                        LiabilitiesDefaultCurrencyBalance = Math.Round(liabilitiesDefaultCurrencyBalance ?? 0, 2),
                        NetWorthUSDBalance = Math.Round((assetsUSDBalance ?? 0) + (liabilitiesUSDBalance ?? 0), 2),
                        NetWorthDefaultCurrencyBalance = Math.Round((assetsDefaultCurrencyBalance ?? 0) + (liabilitiesDefaultCurrencyBalance ?? 0), 2),
                    });
                }
            }

            Entities = new ObservableCollection<ReportStructureSaldoModel>(currentList);
            PlotModel = GetBarChartModel(Entities.ToList());
        }

        protected override string GetSql()
        {
            if (!DateFilter.HasValue)
            {
                MessageBox.Show("Please select date!");
                return string.Empty;
            }

            return string.Format(BaseSqlText, GetStandartTrnFilter());
        }

        private List<DateOnly> GetDatesRange()
        {
            var result = new List<DateOnly>();
            var lastDayOfCurrentMonth = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            if (Range == "Current Year")
            {
                while (true)
                {
                    result.Add(lastDayOfCurrentMonth);
                    if (lastDayOfCurrentMonth.Month == 1)
                        break;
                    lastDayOfCurrentMonth = lastDayOfCurrentMonth.AddMonths(-1);
                }
            }
            else if (Range == "Last 6 Months")
            {
                for (var i = 0; i<6; i++)
                {
                    result.Add(lastDayOfCurrentMonth.AddMonths(-i));
                }
            }
            else if (Range == "Last 12 Months")
            {
                for (var i = 0; i<12; i++)
                {
                    result.Add(lastDayOfCurrentMonth.AddMonths(-i));
                }
            }
            else if (Range == "Last 2 Years")
            {
                while (true)
                {
                    result.Add(lastDayOfCurrentMonth);
                    if (lastDayOfCurrentMonth.Month == 1 && lastDayOfCurrentMonth.Year == DateTime.Today.Year - 1)
                        break;
                    lastDayOfCurrentMonth = lastDayOfCurrentMonth.AddMonths(-1);
                }
            }
            else if (Range == "Last 24 Months")
            {
                for (var i = 0; i<24; i++)
                {
                    result.Add(lastDayOfCurrentMonth.AddMonths(-i));
                }
            }

            return result;
        }

        protected SafePlotModel GetBarChartModel(List<ReportStructureSaldoModel> list)
        {
   
            string currencySymbol = IsUsdCurrencySelected ? "$" : list.FirstOrDefault()?.DefaultCurrencySymbol ?? string.Empty;

            var saldo = new LineSeries
            {
                Title = "Net worth",
                RenderInLegend = true,
                MarkerType = MarkerType.Circle,
                LabelFormatString = "{1}" + currencySymbol,
            };

            var debit = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Category",
                Title = "Liabilities",
                RenderInLegend = true,
                LabelFormatString = "{0}"+ currencySymbol,
            };

            var credit = new BarSeries
            {
                Title = "Assets",
                XAxisKey = "Value",
                YAxisKey = "Category",
                RenderInLegend = true,
                LabelFormatString = "{0}"+ currencySymbol,
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };

            var legend = new Legend
            {
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightMiddle
            };

            var categoryAxis = new CategoryAxis
            {
                StringFormat = "yyyy-MM",
                Position = AxisPosition.Bottom,
                Key = "Category",
                LabelField = "Date",
            };

            var source = list.OrderBy(x => x.Date).ToList();

            categoryAxis.ItemsSource = source;

            int i = 0;
            foreach (var c in source)
            {
                credit.ActualItems.Add(new BarItem { Value = IsUsdCurrencySelected ? c.AssetsUSDBalance ?? 0 : c.AssetsDefaultCurrencyBalance ?? 0 });
                debit.ActualItems.Add(new BarItem { Value = IsUsdCurrencySelected ? c.LiabilitiesUSDBalance ?? 0 : c.LiabilitiesDefaultCurrencyBalance ?? 0 });
                saldo.Points.Add(new DataPoint(i++, IsUsdCurrencySelected ? c.NetWorthUSDBalance ?? 0 : c.NetWorthDefaultCurrencyBalance ?? 0));
            }

            var plotModel = new SafePlotModel();

            plotModel.Series.Add(credit);
            plotModel.Series.Add(debit);
            plotModel.Series.Add(saldo);
            plotModel.Legends.Add(legend);
            plotModel.Axes.Add(valueAxis);
            plotModel.Axes.Add(categoryAxis);
            return plotModel;
        }

        protected override SafePlotModel GetPlotModel(List<ReportStructureSaldoModel> list)
        {
            throw new NotImplementedException();
        }
    }
}