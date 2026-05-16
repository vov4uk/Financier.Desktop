using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    public class ExchangeRateLoader
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IFinancierDatabase _db;
        public ExchangeRateLoader(IFinancierDatabase db)
        {
            _db = db;
        }

        public async Task<List<CurrencyExchangeRate>> LoadExchangeRates()
        {
            var result = new List<CurrencyExchangeRate>();
            var currencies = await _db.ExecuteQuery<CurrencyModel>("select * from currency");

            for (var i = 0; i < currencies.Count; i++)
            {
                for (var j = 0; j < currencies.Count; j++)
                {
                    if (i != j)
                    {
                        var fromCurrency = currencies[i];
                        var toCurrency = currencies[j];
                        var url = buildUrl(fromCurrency.Name, toCurrency.Name);
                        try
                        {
                            using var client = new System.Net.Http.HttpClient();
                            var response = await client.GetAsync(url);
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var (updatedOn, currency, rate) = ParseExchangeRateJson(content);
                                result.Add(new CurrencyExchangeRate
                                {
                                    FromCurrencyId = fromCurrency.Id ?? 0,
                                    ToCurrencyId = toCurrency.Id ?? 0,
                                    Rate = rate,
                                    Date = updatedOn * 1000,
                                    UpdatedOn = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the exception or handle it as needed
                            Logger.Error(ex, $"Error fetching exchange rate from {fromCurrency.Name} to {toCurrency.Name}: {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        private string buildUrl(string fromCurrency, string toCurrency)
        {
            return "https://freecurrencyrates.com/api/action.php?s=fcr&iso=" + toCurrency + "&f=" + fromCurrency + "&v=1&do=cvals";
        }

        public (long UpdatedOn, string Currency, float Rate) ParseExchangeRateJson(string json)
        {
            var obj = JObject.Parse(json);
            var updated = long.Parse(obj["updated"].Value<string>());

            // Find the currency key (any key that's not "updated")
            var currencyProperty = obj.Properties()
                .FirstOrDefault(p => p.Name != "updated");

            return (updated, currencyProperty.Name, currencyProperty.Value.Value<float>());
        }
    }

    public class CurrencyRates
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public long timestamp { get; set; }
        public string @base { get; set; }
        public Dictionary<string, float> rates { get; set; }
    }
}
