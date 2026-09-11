namespace Financier.Desktop.Tests.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Financier.Common.Model;
    using Financier.Desktop.Services;
    using Moq;
    using Moq.Protected;
    using Xunit;

    public class ExchangeRatesServiceTests
    {
        [Fact]
        public async Task LoadFreeCurrencyRates_FailedResponse_ReturnsEmptyList()
        {
            var currencies = new[] { MakeCurrency(985, "PLN"), MakeCurrency(980, "UAH") };
            var service = new ExchangeRatesService(
                CreateHttpClient("error", HttpStatusCode.ServiceUnavailable), () => currencies);

            var result = await service.LoadFreeCurrencyRates();

            Assert.Empty(result);
        }

        [Fact]
        public async Task LoadFreeCurrencyRates_NoCurrencies_ReturnsEmptyList()
        {
            var service = new ExchangeRatesService(
                CreateHttpClient("{}"), () => Array.Empty<CurrencyModel>());

            var result = await service.LoadFreeCurrencyRates();

            Assert.Empty(result);
        }

        [Fact]
        public async Task LoadFreeCurrencyRates_SuccessResponse_ReturnsMappedRates()
        {
            var json = ExtractBodyJson(File.ReadAllText(Path.Combine("Assets", "FreeCurrency.htm")));
            var currencies = new[] { MakeCurrency(985, "PLN"), MakeCurrency(980, "UAH") };
            var service = new ExchangeRatesService(CreateHttpClient(json), () => currencies);

            var result = await service.LoadFreeCurrencyRates();

            Assert.Equal(2, result.Count);
            Assert.All(result, r =>
            {
                Assert.Equal(11.879519f, r.Rate);
                Assert.Equal(1782999003L * 1000L, r.Date);
            });
        }

        [Fact]
        public async Task LoadMonobankRates_FailedResponse_ReturnsEmptyList()
        {
            var currencies = new[] { MakeCurrency(840, "USD"), MakeCurrency(980, "UAH") };
            var service = new ExchangeRatesService(
                CreateHttpClient("error", HttpStatusCode.TooManyRequests), () => currencies);

            var result = await service.LoadMonobankRates();

            Assert.Empty(result);
        }

        [Fact]
        public async Task LoadMonobankRates_NoCurrencies_ReturnsEmptyList()
        {
            var json = File.ReadAllText(Path.Combine("Assets", "mono.json"));
            var service = new ExchangeRatesService(
                CreateHttpClient(json), () => Array.Empty<CurrencyModel>());

            var result = await service.LoadMonobankRates();

            Assert.Empty(result);
        }

        [Fact]
        public async Task LoadMonobankRates_SuccessResponse_ReturnsMappedRate()
        {
            var json = File.ReadAllText(Path.Combine("Assets", "mono.json"));

            // USD (840) → UAH (980) is in mono.json with rateBuy = 44.61
            // UAH → USD has no entry in the index, so it is skipped
            var currencies = new[] { MakeCurrency(840, "USD"), MakeCurrency(980, "UAH") };
            var service = new ExchangeRatesService(CreateHttpClient(json), () => currencies);

            var result = await service.LoadMonobankRates();

            Assert.Single(result);
            Assert.Equal(840, result[0].FromCurrencyId);
            Assert.Equal(980, result[0].ToCurrencyId);
            Assert.Equal(44.61, (double)result[0].Rate, 2);
            Assert.Equal(1782994573L * 1000L, result[0].Date);
        }

        [Fact]
        public async Task LoadOpenExchangeRates_FailedResponse_ReturnsEmptyList()
        {
            var currencies = new[] { MakeCurrency(980, "UAH"), MakeCurrency(985, "PLN") };
            var service = new ExchangeRatesService(
                CreateHttpClient("error", HttpStatusCode.Unauthorized), () => currencies);

            var result = await service.LoadOpenExchangeRates("bad-key");

            Assert.Empty(result);
        }

        [Fact]
        public async Task LoadOpenExchangeRates_SuccessResponse_ReturnsMappedRates()
        {
            var json = File.ReadAllText(Path.Combine("Assets", "openexchangerates.json"));
            var currencies = new[] { MakeCurrency(980, "UAH"), MakeCurrency(985, "PLN") };
            var service = new ExchangeRatesService(CreateHttpClient(json), () => currencies);

            var result = await service.LoadOpenExchangeRates("test-api-key");

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(1782997200L * 1000L, r.Date));

            const float uahRate = 44.806341f;
            const float plnRate = 3.742077f;

            var uahToPln = result.Find(r => r.FromCurrencyId == 980 && r.ToCurrencyId == 985);
            Assert.NotNull(uahToPln);
            Assert.Equal((double)((1.0f / uahRate) * plnRate), (double)uahToPln.Rate, 4);

            var plnToUah = result.Find(r => r.FromCurrencyId == 985 && r.ToCurrencyId == 980);
            Assert.NotNull(plnToUah);
            Assert.Equal((double)((1.0f / plnRate) * uahRate), (double)plnToUah.Rate, 4);
        }

        [Fact]
        public void ParseExchangeRateJson_ValidJson_ReturnsParsedValues()
        {
            var json = ExtractBodyJson(File.ReadAllText(Path.Combine("Assets", "FreeCurrency.htm")));

            var (updatedOn, rate) = ExchangeRatesService.ParseExchangeRateJson(json);

            Assert.Equal(1782999003L, updatedOn);
            Assert.Equal(11.879519f, rate);
        }

        private static HttpClient CreateHttpClient(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseBody),
                });
            return new HttpClient(handler.Object);
        }

        private static string ExtractBodyJson(string html)
        {
            var start = html.IndexOf("<body>", StringComparison.Ordinal) + "<body>".Length;
            var end = html.IndexOf("</body>", start, StringComparison.Ordinal);
            return html[start..end];
        }

        private static CurrencyModel MakeCurrency(int id, string name) =>
            new CurrencyModel { Id = id, Name = name, UpdateExchangeRate = true };
    }
}
