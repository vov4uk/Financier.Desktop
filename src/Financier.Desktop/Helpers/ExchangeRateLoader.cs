using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<List<CurrencyExchangeRate>> LoadFreeCurrencyRates()
        {
            var result = new List<CurrencyExchangeRate>();
            var currencies = await GetRatesPairsAsync(); ;

            foreach (var pair in currencies)
            {
                var fromCurrency = pair.Key;
                var toCurrency = pair.Value;
                var url = buildFreeCurrencyUrl(fromCurrency.Name, toCurrency.Name);
                try
                {
                    using var client = new System.Net.Http.HttpClient();
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var (updatedOn, rate) = ParseExchangeRateJson(content);
                        result.Add(new CurrencyExchangeRate
                        {
                            FromCurrencyId = fromCurrency.Id ?? 0,
                            ToCurrencyId = toCurrency.Id ?? 0,
                            Rate = rate,
                            Date = updatedOn * 1000,
                            UpdatedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        });
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Logger.Warn(content);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Logger.Error(ex, $"Error fetching exchange rate from {fromCurrency.Name} to {toCurrency.Name}: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<List<CurrencyExchangeRate>> LoadOpenExchangeRates(string apiKey)
        {
            var result = new List<CurrencyExchangeRate>();
            var currencies = await GetRatesPairsAsync();
            try
            {
                string url = $"https://openexchangerates.org/api/latest.json?app_id={apiKey}";

                using var client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var exchangeRates = JsonConvert.DeserializeObject<OpenExchangeCurrencyRates>(content);

                    if (exchangeRates?.rates?.Any() == true)
                    {
                        foreach (var pair in currencies)
                        {
                            var fromCurrency = pair.Key;
                            var toCurrency = pair.Value;
                            float fromToUsd = 1.0f / exchangeRates.rates.FirstOrDefault(r => r.Key == fromCurrency.Name).Value;
                            float usdTo = exchangeRates.rates.FirstOrDefault(r => r.Key == toCurrency.Name).Value;

                            result.Add(new CurrencyExchangeRate
                            {
                                FromCurrencyId = fromCurrency.Id ?? 0,
                                ToCurrencyId = toCurrency.Id ?? 0,
                                Rate = fromToUsd * usdTo,
                                Date = exchangeRates.timestamp * 1000,
                                UpdatedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            });
                        }
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.Warn(content);
                }

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Logger.Error(ex, $"Error fetching exchange rate: {ex.Message}");
            }

            return result;
        }

        private async Task<List<KeyValuePair<CurrencyModel, CurrencyModel>>> GetRatesPairsAsync()
        {
            var result = new List<KeyValuePair<CurrencyModel, CurrencyModel>>();
            var currencies = await _db.ExecuteQuery<CurrencyModel>("select * from currency");

            for (var i = 0; i < currencies.Count; i++)
            {
                for (var j = 0; j < currencies.Count; j++)
                {
                    if (i != j)
                    {
                        var fromCurrency = currencies[i];
                        var toCurrency = currencies[j];
                        result.Add(new KeyValuePair<CurrencyModel, CurrencyModel>(fromCurrency, toCurrency));
                    }
                }
            }
            return result;
        }

        private static string buildFreeCurrencyUrl(string fromCurrency, string toCurrency)
        {
            return "https://freecurrencyrates.com/api/action.php?s=fcr&iso=" + toCurrency + "&f=" + fromCurrency + "&v=1&do=cvals";
        }

        public static (long UpdatedOn, float Rate) ParseExchangeRateJson(string json)
        {
            var obj = JObject.Parse(json);
            var updated = long.Parse(obj["updated"].Value<string>());

            // Find the currency key (any key that's not "updated")
            var currencyProperty = obj.Properties()
                .FirstOrDefault(p => p.Name != "updated");

            return (updated, currencyProperty.Value.Value<float>());
        }
    }

    public class OpenExchangeCurrencyRates
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public long timestamp { get; set; }
        public string @base { get; set; }
        public Dictionary<string, float> rates { get; set; }
    }
}
