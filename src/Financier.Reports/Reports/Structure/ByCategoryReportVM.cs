using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using Financier.DataAccess.Utils;
using Financier.Reports.Controls;
using Financier.Reports.Converters;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Financier.Reports.Common;

namespace Financier.Reports.Reports
{
    [Header("By category")]
    [ExcludeFromCodeCoverage]
    public class ByCategoryReportVM : BindableBase
    {
        private readonly IFinancierDatabase financierDatabase;
        private DelegateCommand _refreshCommand;

        private double currentWidth;

        private DateTime from;

        private PeriodType periodType;

        private BarChart plot;

        private DateTime to;

        public string Header { get; set; }

        public ByCategoryReportVM(IFinancierDatabase financierDatabase)
        {
            this.financierDatabase = financierDatabase; 
            PeriodType = PeriodType.Today;
            UpdatePeriod(PeriodType);
        }


        public DateTime From
        {
            get => from;
            set
            {
                if (SetProperty(ref from, value))
                {
                    RaisePropertyChanged(nameof(From));
                }
            }
        }

        public PeriodType PeriodType
        {
            get => periodType;
            set
            {
                if (SetProperty(ref periodType, value))
                {
                    RaisePropertyChanged(nameof(PeriodType));
                    UpdatePeriod(value);
                }
            }
        }

        public BarChart Plot
        {
            get => plot;
            set
            {
                if (SetProperty(ref plot, value))
                {
                    RaisePropertyChanged(nameof(Plot));
                }
            }
        }

        public DelegateCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??= new DelegateCommand(() => RefreshReport(currentWidth));
            }
        }
        public DateTime To
        {
            get => to;
            set
            {
                if (SetProperty(ref to, value))
                {
                    RaisePropertyChanged(nameof(To));
                }
            }
        }

        // TODO - bug, transaction fron top-level categories not included into ByCategoryReportV2
        // use another view or rewrite to SQL query as others reports
        internal async void RefreshReport(double width)
        {
            using var uow = financierDatabase.CreateUnitOfWork();
            var allCategories = await uow.GetAllAsync<Category>();
            var orderedCategories = allCategories.Where(x => x.Id > 0).OrderBy(x => x.Left).ToList();
            var repo = uow.GetRepository<ByCategoryReportV2>();
            var fromUnix = UnixTimeConverter.ConvertBack(From);
            var toUnix = UnixTimeConverter.ConvertBack(To);
            var entites = await repo.FindManyAsync(
                y => y.datetime >= fromUnix && y.datetime <= toUnix,
                x => x.from_account_currency,
                x => x.to_account_currency,
                x => x.category );

            currentWidth = width;

            var filteredReportValues = entites
                .GroupBy(x => x.Id)
                .Select(x => new ByCategoryReportRow
                {
                   Id = x.Key,
                   Title = x.FirstOrDefault().name,
                   Left = x.FirstOrDefault().category_left,
                   Right = x.FirstOrDefault().category_right,
                   CurrencySign = x.FirstOrDefault().from_account_currency.Symbol,
                   SubCategoties = new List<ByCategoryReportRow>(),
                   TotalPositiveAmount = x.Where(y => y.from_amount_default_currency > 0).Sum(y => y.from_amount_default_currency),
                   TotalNegativeAmount = x.Where(y => y.from_amount_default_currency < 0).Sum(y => y.from_amount_default_currency),
                }).ToList();

            var finalTree = new List<ByCategoryReportRow>();

            BuildByCategoryReportTree(finalTree, orderedCategories, filteredReportValues, 0);
            var barChartSource = finalTree.Where(x => x.GetAbsoluteMax() > 0).OfType<ReportRow>().ToList();
            if (barChartSource.Count > 0)
            {
                Plot = new BarChart(barChartSource, width - 80); // 80px label width
            }
            else
            {
                Plot = default;
            }
        }

        private void BuildByCategoryReportTree(List<ByCategoryReportRow> newNodes, List<Category> categories, List<ByCategoryReportRow> groupedNodes, int level)
        {
            foreach (var category in categories.OrderBy(x => x.Left))
            {
                if (!newNodes.Any(x => x.Right > category.Left))
                {
                    var subNode = new ByCategoryReportRow
                    {
                        Id = category.Id,
                        Title = category.Title,
                        Left = category.Left,
                        Right = category.Right,
                        TotalNegativeAmount = 0,
                        TotalPositiveAmount = 0,
                        SubCategoties = new List<ByCategoryReportRow>()
                    };

                    var nodeWithAmount = groupedNodes.FirstOrDefault(x => x.Id == category.Id);
                    if (nodeWithAmount != null)
                    {
                        subNode.TotalNegativeAmount = nodeWithAmount.TotalNegativeAmount;
                        subNode.TotalPositiveAmount = nodeWithAmount.TotalPositiveAmount;
                        subNode.CurrencySign = nodeWithAmount.CurrencySign;
                    }

                    newNodes.Add(subNode);

                    var sub = categories.Where(x => x.Left > category.Left && x.Right < category.Right).ToList();
                    if (sub.Any())
                    {
                        BuildByCategoryReportTree(subNode.SubCategoties, sub, groupedNodes, level + 1);
                        if (level == 0)
                        {
                            foreach (var item in subNode.SubCategoties)
                            {
                                subNode.TotalNegativeAmount += item.TotalNegativeAmount;
                                subNode.TotalPositiveAmount += item.TotalPositiveAmount;
                            }
                        }

                        subNode.CurrencySign = subNode.SubCategoties.FirstOrDefault(x => !string.IsNullOrEmpty(x.CurrencySign))?.CurrencySign;
                    }
                }
            }
        }

        private void UpdatePeriod(PeriodType type)
        {
            switch (type)
            {
                case PeriodType.Custom:
                    break;
                case PeriodType.Today:
                    {
                        From = DateTime.Today;
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.Yesterday:
                    {
                        From = DateTime.Today.AddDays(-1);
                        To = DateTime.Today.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate.AddDays(-7);
                        To = startingDate.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        To = new DateTime(today.Year, today.Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate;
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.Year, today.Month, 1);
                        To = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate.AddDays(-7);
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        To = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
