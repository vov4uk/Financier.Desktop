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

        public async Task<List<CurrencyExchangeRate>> LoadRates()
        {
//1   UAH
//2   USD
//3   EUR
//4   PLN

            var result = new List<CurrencyExchangeRate>();
            var files = Directory.GetFiles(@"C:\FFOutput\Rates", "*.json");

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var rates = JsonConvert.DeserializeObject<CurrencyRates>(json);

                result.Add(new CurrencyExchangeRate
                {
                    FromCurrencyId = 3,
                    ToCurrencyId = 2,
                    Rate = 1/rates.rates["EUR"],
                    Date = rates.timestamp * 1000,
                });
                result.Add(new CurrencyExchangeRate
                {
                    FromCurrencyId = 1,
                    ToCurrencyId = 2,
                    Rate = 1 / rates.rates["UAH"],
                    Date = rates.timestamp * 1000
                });
                result.Add(new CurrencyExchangeRate
                {
                    FromCurrencyId = 4,
                    ToCurrencyId = 2,
                    Rate = 1 / rates.rates["PLN"],
                    Date = rates.timestamp * 1000
                });
            }

            var nbuJson = await File.ReadAllTextAsync(@"C:\FFOutput\currency_rates.json");
            var nbuRates = JsonConvert.DeserializeObject<List<rurrencyRates>>(nbuJson);

            var startDate = new DateTime(2026, 5, 1);
            for (int i = 0; i < 150; i++)
            {
                var currentDate = startDate.AddMonths(-1 * i);
                var currentDateString = currentDate.ToString("dd.MM.yyyy");
                long unixTime = ((DateTimeOffset)currentDate).ToUnixTimeSeconds() * 1000;

                var pln = nbuRates.FirstOrDefault(x => x.date == currentDateString && x.letter_code == "PLN");
                var eur = nbuRates.FirstOrDefault(x => x.date == currentDateString && x.letter_code == "EUR");

                if (pln != null)
                {
                    float plnRate = pln.official_rate.Value / pln.quantity.Value;

                    result.Add(new CurrencyExchangeRate
                    {
                        FromCurrencyId = 1,
                        ToCurrencyId = 4,
                        Rate = 1 / plnRate,
                        Date = unixTime
                    });
                    result.Add(new CurrencyExchangeRate
                    {
                        FromCurrencyId = 4,
                        ToCurrencyId = 1,
                        Rate = plnRate,
                        Date = unixTime
                    });
                }

                if (eur != null)
                {
                    float eurRate = eur.official_rate.Value / eur.quantity.Value;
                    result.Add(new CurrencyExchangeRate
                    {
                        FromCurrencyId = 1,
                        ToCurrencyId = 3,
                        Rate = 1 / eurRate,
                        Date = unixTime
                    });
                    result.Add(new CurrencyExchangeRate
                    {
                        FromCurrencyId = 3,
                        ToCurrencyId = 1,
                        Rate = eurRate,
                        Date = unixTime
                    });
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

    [DebuggerDisplay("{date}--{official_rate}")]
    public class rurrencyRates
    {
        //Дата,Час,Код цифровий, Код літерний,Кількість одиниць, Назва валюти,"Офіційний курс гривні, грн",Примітка

        public string date { get; set; }

        public int code { get; set; }

        public string letter_code { get; set; }

        public int? quantity { get; set; }

        public string currency_name { get; set; }

        public float? official_rate { get; set; }

        public string note { get; set; }
    }
}
