using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.Converters;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Localization;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ExchangeRatesVM : EntityBaseVM<ExchangeRateModel>
    {
        private CurrencyModel _from;
        private CurrencyModel _to;

        private PlotModel plotModel;

        public ExchangeRatesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper, LocalizationManager localizationManager)
            : base(db, dialogWrapper, localizationManager)
        {
        }

        public CurrencyModel From
        {
            get => _from;
            set
            {
                if (SetProperty(ref _from, value))
                {
                    RaisePropertyChanged(nameof(From));
                    RaisePropertyChanged(nameof(ToCurrencies));
                }
            }
        }

        public IEnumerable<CurrencyModel> FromCurrencies =>
            DbManual.Currencies.Where(x => x.Id > 0);

        public PlotModel PlotModel
        {
            get => plotModel;
            private set
            {
                plotModel = value;
                RaisePropertyChanged(nameof(PlotModel));
            }
        }

        public CurrencyModel To
        {
            get => _to;
            set
            {
                if (SetProperty(ref _to, value))
                {
                    RaisePropertyChanged(nameof(To));
                }
            }
        }
        public IEnumerable<CurrencyModel> ToCurrencies =>
            DbManual.Currencies.Where(x => x.Id > 0 && x.Id != _from?.Id);
        protected override Task OnAdd() => throw new NotImplementedException();

        protected override Task OnDelete(ExchangeRateModel item) => throw new NotImplementedException();

        protected override Task OnEdit(ExchangeRateModel item) => throw new NotImplementedException();

        protected override async Task RefreshData()
        {
            using var uow = db.CreateUnitOfWork();
            var currencyExchangeRepo = uow.GetRepository<CurrencyExchangeRate>();
            var items = await currencyExchangeRepo.FindManyAndProjectAsync(
                x => x.FromCurrencyId == (_from != null ? _from.Id : 0) && x.ToCurrencyId == (_to != null ? _to.Id : 0), // where
                rate => new ExchangeRateModel
                {
                    Date = rate.Date,
                    ToCurrencyId = rate.ToCurrencyId,
                    FromCurrencyId = rate.FromCurrencyId,
                    Rate = rate.Rate,
                    FromCurrency = new CurrencyModel
                    {
                        Id = rate.FromCurrency.Id,
                        Name = rate.FromCurrency.Name,
                        Symbol = rate.FromCurrency.Symbol,
                    },
                    ToCurrency = new CurrencyModel
                    {
                        Id = rate.ToCurrency.Id,
                        Name = rate.ToCurrency.Name,
                        Symbol = rate.ToCurrency.Symbol,
                    },
                }, // projection
                x => x.FromCurrency, // inclide
                x => x.ToCurrency // include
                );

            if (items == null)
            {
                return;
            }
            Entities = new ObservableCollection<ExchangeRateModel>(items.OrderByDescending(x => x.Date));

            var model = new PlotModel();
            var dateTimeAxis = new DateTimeAxis();

            var linearAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };
            var lineSeries = new LineSeries
            {
                Color = OxyColor.FromArgb(255, 78, 154, 6),
                MarkerFill = OxyColor.FromArgb(255, 78, 154, 6),
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Plus,
                StrokeThickness = 1
            };

            foreach (var item in items.OrderBy(x => x.Date))
            {
                var date = UnixTimeConverter.Convert(item.Date);
                lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(date, item.Rate));
            }

            model.Axes.Add(dateTimeAxis);
            model.Axes.Add(linearAxis);
            model.Series.Add(lineSeries);

            PlotModel = model;

        }
    }
}
