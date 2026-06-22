namespace Financier.Desktop.Tests.Wizards.Mono.Revolut
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Financier.Desktop.Helpers.BankHelper;
    using Financier.Desktop.Wizards;
    using Xunit;

    public class RevolutHelperTest
    {
        [Fact]
        public void ParseReport_FileDoesNotExist_ReturnsEmpty()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", Guid.NewGuid().ToString());
            IEnumerable<BankTransaction> result = new RevolutHelper().ParseReport(path);

            Assert.Empty(result);
        }

        [Fact]
        public void ParseReport_ValidCsv_ReturnsExpectedCount()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.pl.csv");
            IEnumerable<BankTransaction> result = new RevolutHelper().ParseReport(path);

            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void ParseReport_ValidCsv_FirstRowMappedCorrectly()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.pl.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            var first = result[0];
            Assert.Equal(new DateTime(2026, 4, 24, 13, 46, 7), first.Date);
            Assert.Equal("Zasilenie {xPay} za pomocą {card}", first.Description);
            Assert.Equal(50.00, first.CardCurrencyAmount);
            Assert.Equal(50.00, first.OperationAmount);
            Assert.Equal("PLN", first.OperationCurrency);
            Assert.Equal(0.00, first.Commission);
            Assert.Equal(50.00, first.Balance);
        }

        [Fact]
        public void ParseReport_ValidCsv_NegativeAmountMappedCorrectly()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.pl.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            var mpk = result[1];
            Assert.Equal(new DateTime(2026, 4, 28, 1, 24, 7), mpk.Date);
            Assert.Equal("MPK", mpk.Description);
            Assert.Equal(-3.20, mpk.CardCurrencyAmount);
            Assert.Equal(46.80, mpk.Balance);
        }

        [Fact]
        public void ParseReport_PendingTransaction_BalanceDefaultsToZero()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.pl.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            var pending = result[3];
            Assert.Equal("Urbancard", pending.Description);
            Assert.Equal(-3.20, pending.CardCurrencyAmount);
            Assert.Equal(0.0, pending.Balance);
        }

        [Fact]
        public void ParseReport_EnglishCsv_ReturnsExpectedCount()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.en.csv");
            var result = new RevolutHelper().ParseReport(path);

            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void ParseReport_EnglishCsv_SingleDigitHour_ParsedCorrectly()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.en.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            Assert.Contains(result, r => r.Date == new DateTime(2026, 4, 30, 1, 26, 17));
            Assert.Contains(result, r => r.Date == new DateTime(2026, 5, 1, 1, 16, 19));
            Assert.Contains(result, r => r.Date == new DateTime(2026, 5, 2, 0, 2, 28));
        }

        [Fact]
        public void ParseReport_EnglishCsv_ExchangeDebit_MappedCorrectly()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.en.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            var exchange = result.First(r => r.Description == "Transfer to Revolut Digital Assets Europe Ltd");
            Assert.Equal(new DateTime(2026, 5, 8, 11, 13, 59), exchange.Date);
            Assert.Equal(-3.64, exchange.CardCurrencyAmount);
            Assert.Equal(-3.64, exchange.OperationAmount);
            Assert.Equal("PLN", exchange.OperationCurrency);
            Assert.Equal(0.0, exchange.Commission);
            Assert.Equal(33.56, exchange.Balance);
        }

        [Fact]
        public void ParseReport_EnglishCsv_ExchangeCredit_PositiveAmount()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "revolut.en.csv");
            var result = new RevolutHelper().ParseReport(path).ToList();

            var credit = result.First(r => r.Description == "Transfer from Revolut Digital Assets Europe Ltd");
            Assert.Equal(3.54, credit.CardCurrencyAmount);
            Assert.Equal(37.1, credit.Balance);
        }
    }
}
