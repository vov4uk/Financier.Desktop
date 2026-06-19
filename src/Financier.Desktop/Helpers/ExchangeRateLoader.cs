using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Financier.Desktop.Helpers
{
    public class ExchangeRateLoader
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public ExchangeRateLoader()
        {
        }

        public async Task<List<CurrencyExchangeRate>> LoadFreeCurrencyRates()
        {
            var result = new List<CurrencyExchangeRate>();
            var currencies = GetRatesPairs();

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

        public async Task<List<CurrencyExchangeRate>> LoadOpenExchangeRates(string encryptedApiKey)
        {
            var result = new List<CurrencyExchangeRate>();
            var currencies = GetRatesPairs();
            try
            {
                string apiKey = SettingsProtection.TryDecrypt(encryptedApiKey);

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

        private static List<KeyValuePair<CurrencyModel, CurrencyModel>> GetRatesPairs()
        {
            var result = new List<KeyValuePair<CurrencyModel, CurrencyModel>>();
            var currencies = DbManual.Currencies;

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
                string monoUrl = "api.monobank.ua/bank/currency";
                var currencies = GetRatesPairs();

                using var client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync($"https://{monoUrl}");
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"Monobank API returned {response.StatusCode}");
                    return result;
                }

                var content = await response.Content.ReadAsStringAsync();
                var rates = JsonConvert.DeserializeObject<List<MonobankRate>>(content);
                var updatedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var rateIndex = (rates ?? [])
                    .Where(r => r.RateBuy > 0 || r.RateCross > 0)
                    .ToDictionary(r => (r.CurrencyCodeA, r.CurrencyCodeB));

                foreach (var pair in currencies)
                {
                    var fromCurrency = pair.Key;
                    var toCurrency = pair.Value;

                    if (!AlphaToNumeric.TryGetValue(fromCurrency.Name, out var fromCode) ||
                        !AlphaToNumeric.TryGetValue(toCurrency.Name, out var toCode))
                        continue;

                    if (!rateIndex.TryGetValue((fromCode, toCode), out var rate))
                        continue;

                    var exchangeRate = rate.RateBuy > 0 ? rate.RateBuy : rate.RateCross;

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

        private static Dictionary<string, int> AlphaToNumeric = new Dictionary<string, int>
        {
            { "AFN", 971 },
            { "EUR", 978 },
            { "ALL", 8 },
            { "DZD", 12 },
            { "USD", 840 },
            { "AOA", 973 },
            { "XCD", 951 },
            { "XAD", 396 },
            { "ARS", 32 },
            { "AMD", 51 },
            { "AWG", 533 },
            { "AUD", 36 },
            { "AZN", 944 },
            { "BSD", 44 },
            { "BHD", 48 },
            { "BDT", 50 },
            { "BBD", 52 },
            { "BYN", 933 },
            { "BZD", 84 },
            { "XOF", 952 },
            { "BMD", 60 },
            { "INR", 356 },
            { "BTN", 64 },
            { "BOB", 68 },
            { "BOV", 984 },
            { "BAM", 977 },
            { "BWP", 72 },
            { "NOK", 578 },
            { "BRL", 986 },
            { "BND", 96 },
            { "BIF", 108 },
            { "CVE", 132 },
            { "KHR", 116 },
            { "XAF", 950 },
            { "CAD", 124 },
            { "KYD", 136 },
            { "CLP", 152 },
            { "CLF", 990 },
            { "CNY", 156 },
            { "COP", 170 },
            { "COU", 970 },
            { "KMF", 174 },
            { "CDF", 976 },
            { "NZD", 554 },
            { "CRC", 188 },
            { "CUP", 192 },
            { "XCG", 532 },
            { "CZK", 203 },
            { "DKK", 208 },
            { "DJF", 262 },
            { "DOP", 214 },
            { "EGP", 818 },
            { "SVC", 222 },
            { "ERN", 232 },
            { "SZL", 748 },
            { "ETB", 230 },
            { "FKP", 238 },
            { "FJD", 242 },
            { "XPF", 953 },
            { "GMD", 270 },
            { "GEL", 981 },
            { "GHS", 936 },
            { "GIP", 292 },
            { "GTQ", 320 },
            { "GBP", 826 },
            { "GNF", 324 },
            { "GYD", 328 },
            { "HTG", 332 },
            { "HNL", 340 },
            { "HKD", 344 },
            { "HUF", 348 },
            { "ISK", 352 },
            { "IDR", 360 },
            { "XDR", 960 },
            { "IRR", 364 },
            { "IQD", 368 },
            { "ILS", 376 },
            { "JMD", 388 },
            { "JPY", 392 },
            { "JOD", 400 },
            { "KZT", 398 },
            { "KES", 404 },
            { "KPW", 408 },
            { "KRW", 410 },
            { "KWD", 414 },
            { "KGS", 417 },
            { "LAK", 418 },
            { "LBP", 422 },
            { "LSL", 426 },
            { "ZAR", 710 },
            { "LRD", 430 },
            { "LYD", 434 },
            { "CHF", 756 },
            { "MOP", 446 },
            { "MKD", 807 },
            { "MGA", 969 },
            { "MWK", 454 },
            { "MYR", 458 },
            { "MVR", 462 },
            { "MRU", 929 },
            { "MUR", 480 },
            { "XUA", 965 },
            { "MXN", 484 },
            { "MXV", 979 },
            { "MDL", 498 },
            { "MNT", 496 },
            { "MAD", 504 },
            { "MZN", 943 },
            { "MMK", 104 },
            { "NAD", 516 },
            { "NPR", 524 },
            { "NIO", 558 },
            { "NGN", 566 },
            { "OMR", 512 },
            { "PKR", 586 },
            { "PAB", 590 },
            { "PGK", 598 },
            { "PYG", 600 },
            { "PEN", 604 },
            { "PHP", 608 },
            { "PLN", 985 },
            { "QAR", 634 },
            { "RON", 946 },
            { "RUB", 643 },
            { "RWF", 646 },
            { "SHP", 654 },
            { "WST", 882 },
            { "STN", 930 },
            { "SAR", 682 },
            { "RSD", 941 },
            { "SCR", 690 },
            { "SLE", 925 },
            { "SGD", 702 },
            { "XSU", 994 },
            { "SBD", 90 },
            { "SOS", 706 },
            { "SSP", 728 },
            { "LKR", 144 },
            { "SDG", 938 },
            { "SRD", 968 },
            { "SEK", 752 },
            { "CHE", 947 },
            { "CHW", 948 },
            { "SYP", 760 },
            { "TWD", 901 },
            { "TJS", 972 },
            { "TZS", 834 },
            { "THB", 764 },
            { "TOP", 776 },
            { "TTD", 780 },
            { "TND", 788 },
            { "TRY", 949 },
            { "TMT", 934 },
            { "UGX", 800 },
            { "UAH", 980 },
            { "AED", 784 },
            { "USN", 997 },
            { "UYU", 858 },
            { "UYI", 940 },
            { "UYW", 927 },
            { "UZS", 860 },
            { "VUV", 548 },
            { "VES", 928 },
            { "VED", 926 },
            { "VND", 704 },
            { "YER", 886 },
            { "ZMW", 967 },
            { "ZWG", 924 },
            { "XBA", 955 },
            { "XBB", 956 },
            { "XBC", 957 },
            { "XBD", 958 },
            { "XTS", 963 },
            { "XXX", 999 },
            { "XAU", 959 },
            { "XPD", 964 },
            { "XPT", 962 },
            { "XAG", 961 }
        };

        private static string buildFreeCurrencyUrl(string fromCurrency, string toCurrency)
        {
            return "https://freecurrencyrates.com/api/action.php?s=fcr&iso=" + toCurrency + "&f=" + fromCurrency + "&v=1&do=cvals";
        }

        public static (long UpdatedOn, float Rate) ParseExchangeRateJson(string json)
        {
            var obj = JObject.Parse(json);
            var updated = long.Parse(obj["updated"]!.Value<string>()!);

            // Find the currency key (any key that's not "updated")
            var currencyProperty = obj.Properties()
                .FirstOrDefault(p => p.Name != "updated");

            return (updated, currencyProperty!.Value.Value<float>());
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
}
