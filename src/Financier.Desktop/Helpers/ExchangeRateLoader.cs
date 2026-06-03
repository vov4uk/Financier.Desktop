using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
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

        public async Task<List<CurrencyExchangeRate>> LoadMonobankRates()
        {
            var result = new List<CurrencyExchangeRate>();
            try
            {
                var numericToAlpha = LoadIso4217Map();
                var dbCurrencies = await _db.ExecuteQuery<CurrencyModel>("select * from currency");
                var dbByName = dbCurrencies
                    .Where(c => !string.IsNullOrEmpty(c.Name))
                    .ToDictionary(c => c.Name, c => c);

                using var client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Financier.Desktop");
                var response = await client.GetAsync("https://api.monobank.ua/bank/currency");
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"Monobank API returned {response.StatusCode}");
                    return result;
                }

                var content = await response.Content.ReadAsStringAsync();
                var rates = JsonConvert.DeserializeObject<List<MonobankRate>>(content);
                var updatedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                foreach (var rate in rates ?? [])
                {
                    if (!numericToAlpha.TryGetValue(rate.CurrencyCodeA, out var fromAlpha) ||
                        !numericToAlpha.TryGetValue(rate.CurrencyCodeB, out var toAlpha))
                        continue;

                    if (!dbByName.TryGetValue(fromAlpha, out var fromCurrency) ||
                        !dbByName.TryGetValue(toAlpha, out var toCurrency))
                        continue;

                    var exchangeRate = rate.RateBuy > 0 ? rate.RateBuy : rate.RateCross;
                    if (exchangeRate <= 0)
                        continue;

                    result.Add(new CurrencyExchangeRate
                    {
                        FromCurrencyId = fromCurrency.Id ?? 0,
                        ToCurrencyId = toCurrency.Id ?? 0,
                        Rate = (float)exchangeRate,
                        Date = rate.Date * 1000,
                        UpdatedOn = updatedOn
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error fetching Monobank exchange rates: {ex.Message}");
            }

            return result;
        }

        private static Dictionary<int, string> LoadIso4217Map()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("Financier.Desktop.Assets.iso4217.csv");
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<Iso4217Record>()
                .Where(r => !string.IsNullOrEmpty(r.AlphabeticCode) && !string.IsNullOrEmpty(r.NumericCode)
                            && int.TryParse(r.NumericCode, out _)
                            && string.IsNullOrEmpty(r.WithdrawalDate))
                .GroupBy(r => int.Parse(r.NumericCode))
                .ToDictionary(g => g.Key, g => g.First().AlphabeticCode);
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

    public class MonobankRate
    {
        [JsonProperty("currencyCodeA")]
        public int CurrencyCodeA { get; set; }

        [JsonProperty("currencyCodeB")]
        public int CurrencyCodeB { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("rateBuy")]
        public double RateBuy { get; set; }

        [JsonProperty("rateSell")]
        public double RateSell { get; set; }

        [JsonProperty("rateCross")]
        public double RateCross { get; set; }
    }

    public class Iso4217Record
    {
        [Name("Entity")]
        public string Entity { get; set; }

        [Name("Currency")]
        public string Currency { get; set; }

        [Name("AlphabeticCode")]
        public string AlphabeticCode { get; set; }

        [Name("NumericCode")]
        public string NumericCode { get; set; }

        [Name("MinorUnit")]
        public string MinorUnit { get; set; }

        [Name("WithdrawalDate")]
        public string WithdrawalDate { get; set; }
    }
}
