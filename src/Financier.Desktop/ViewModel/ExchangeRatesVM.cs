using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ExchangeRatesVM : EntityBaseVM<ExchangeRateModel>
    {
        public ExchangeRatesVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override async Task RefreshData()
        {
            using var uow = financierDatabase.CreateUnitOfWork();
            var accountRepo = uow.GetRepository<CurrencyExchangeRate>();
            var items = await accountRepo.FindManyAsync(
                x => true, // where
                rate => new ExchangeRateModel
                {
                    Date = rate.Date,
                    ToCurrencyId = rate.ToCurrencyId,
                    FromCurrencyId = rate.FromCurrencyId,
                    Rate = rate.Rate,
                    FromCurrency = new CurrencyModel
                    {
                        Id = rate.FromCurrency.Id,
                        IsDefault = rate.FromCurrency.IsDefault ? 1 : 0,
                        Name = rate.FromCurrency.Name,
                        Symbol = rate.FromCurrency.Symbol,
                    },
                    ToCurrency = new CurrencyModel
                    {
                        Id = rate.ToCurrency.Id,
                        IsDefault = rate.ToCurrency.IsDefault ? 1 : 0,
                        Name = rate.ToCurrency.Name,
                        Symbol = rate.ToCurrency.Symbol,
                    },
                }, // projection
                x => x.FromCurrency, // inclide
                x => x.ToCurrency // include
                );

            Entities = new ObservableCollection<ExchangeRateModel>(items);
        }
    }
}
