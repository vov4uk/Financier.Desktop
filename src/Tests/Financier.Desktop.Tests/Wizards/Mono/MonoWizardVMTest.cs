namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Data;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Newtonsoft.Json;
    using Xunit;

    public class MonoWizardVMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveParameters_CurrentPageNotEmpty(List<BankTransaction> mono)
        {
            DbManual.SetupTests(new List<AccountFilterModel>());
            var vm = new MonoWizardVM("Monobank", mono, new Dictionary<int, BlotterModel>());

            Assert.NotNull(vm.CurrentPage);
        }

        [Fact]
        public void LoadTransactions_UkrHeaders_TransactionsLoaded()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            var mono = new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM("Monobank", mono, new Dictionary<int, BlotterModel>());

            Assert.Equal(46, mono.Count());
            Assert.Equal(46, ((Page2VM)vm.Pages[1]).GetMonoTransactions().Count);
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Fact]
        public void LoadTransactions_EngHeaders_TransactionsLoaded()
        {
            DbManual.SetupTests(new List<AccountFilterModel>());
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.eng.csv");
            IEnumerable<BankTransaction> mono = new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM("Monobank", mono, new Dictionary<int, BlotterModel>());

            Assert.Single(((Page2VM)vm.Pages[1]).GetMonoTransactions());
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Fact]
        public void LoadTransactions_Monobank_ExpectedTransactions()
        {
            var expected = new List<BankTransaction>
            {
               new BankTransaction
               {
                    Date = new DateTime(2021, 11, 15, 10, 19, 11, DateTimeKind.Local),
                    Description = "Київстар",
                    Balance = 0.0,
                    MCC = "4814",
                    CardCurrencyAmount = -132.25,
                    OperationAmount = -132.25,
                    OperationCurrency = "UAH",
               },
            };

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.eng.csv");
            IEnumerable<BankTransaction> mono = new Helpers.MonobankHelper().ParseReport(csvPath);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(mono.ToList()));
        }

        [Fact]
        public void LoadTransactions_Monobank_EmptyList()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", Guid.NewGuid().ToString());
            IEnumerable<BankTransaction> mono = new Helpers.MonobankHelper().ParseReport(csvPath);

            Assert.Empty(mono);
        }

        [Fact]
        public void LoadTransactions_AbankMultiPages_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2024, 4, 30, 19, 41, 0),
                Description = "Монобанк",
                Balance = 2072.28,
                MCC = "6010",
                Commission = 0.0,
                CardCurrencyAmount = 2000.0,
                OperationAmount = 2000.0,
                Cashback = 0.0,
                ExchangeRate = 0.0
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2024, 4, 4, 9, 51, 0),
                Description = "Монобанк",
                Balance = 3166.91,
                Commission = 0.0,
                MCC = "6010",
                CardCurrencyAmount = 2000.0,
                OperationAmount = 2000.0,
                Cashback = 0.0,
                ExchangeRate = 0.0
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "abank_3_pages.pdf");
            IEnumerable<BankTransaction> abank = new Helpers.ABankHelper().ParseReport(path);

            Assert.Equal(17, abank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(abank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(abank.Last()));
        }

        [Fact]
        public void LoadTransactions_AbankExcel_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2024, 4, 30, 19, 41, 0),
                Description = "Монобанк",
                Balance = 2072.28,
                MCC = "6010",
                Commission = 0.0,
                CardCurrencyAmount = 2000.0,
                OperationAmount = 2000.0,
                Cashback = 0.0,
                ExchangeRate = 0.0
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2024, 4, 4, 9, 51, 0),
                Description = "Монобанк",
                Balance = 3166.91,
                Commission = 0.0,
                MCC = "6010",
                CardCurrencyAmount = 2000.0,
                OperationAmount = 2000.0,
                Cashback = 0.0,
                ExchangeRate = 0.0
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "abank.xlsx");
            IEnumerable<BankTransaction> abank = new Helpers.AbankExcelHelper().ParseReport(path);

            Assert.Equal(17, abank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(abank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(abank.Last()));
        }

        [Fact]
        public void LoadTransactions_AbankEnglishExcel_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2024, 6, 21, 18, 29, 0),
                Description = "Exchange. Rate 40.20",
                Balance = 894.72,
                MCC = "4829",
                Commission = 0.0,
                CardCurrencyAmount = 51.85,
                OperationAmount = 51.85,
                Cashback = 0.0,
                ExchangeRate = 0.0
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2024, 5, 24, 12, 44, 0),
                Description = "ATB",
                Balance = 2306.27,
                Commission = 0.0,
                MCC = "5411",
                CardCurrencyAmount = -472.92,
                OperationAmount = -472.92,
                Cashback = 5.67,
                ExchangeRate = 0.0
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "abank.eng.xlsx");
            IEnumerable<BankTransaction> abank = new Helpers.AbankExcelHelper().ParseReport(path);

            Assert.Equal(26, abank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(abank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(abank.Last()));
        }

        [Fact]
        public void LoadTransactions_Pireus_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2024, 5, 27, 15, 13, 41),
                Description = "Оплата покупки (magazyn Rodynna kovbaska(P0043515) m.Lviv UA)",
                Balance = 132.63,
                MCC = null,
                Commission = 0.0,
                CardCurrencyAmount = -209.68,
                OperationAmount = -209.68,
                Cashback = null,
                ExchangeRate = null
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2024, 4, 18, 9, 49, 2),
                Description = "Оплата покупки (magazyn Rodynna kovbaska(P0043515) m.Lviv UA)",
                Balance = 0.0,
                Commission = 0.0,
                MCC = null,
                CardCurrencyAmount = -22.9,
                OperationAmount = -22.9,
                Cashback = null,
                ExchangeRate = null
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "pireus.pdf");
            IEnumerable<BankTransaction> pireus = new Helpers.PireusHelper().ParseReport(path);

            Assert.Equal(111, pireus.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(pireus.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(pireus.Last()));
        }

        [Fact]
        public void LoadTransactions_Pumb_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2023, 04, 25, 9, 10, 6),
                Description = "Переказ ACCOUNT Списання",
                CardCurrencyAmount = -174.0,
                OperationAmount = -174.0,
                OperationCurrency = "UAH",
                Commission = 0.0,
                Balance = 0.0
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2023, 4, 11, 22, 19, 10),
                Description = "Чистий дохід від підприємницької діяльн Надходження",
                CardCurrencyAmount = 9372.16,
                OperationAmount = 9372.16,
                OperationCurrency = "UAH",
                Commission = 0.0,
                Balance = 0.0
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "pumb.pdf");
            IEnumerable<BankTransaction> bank = new Helpers.PumbHelper().ParseReport(path);

            Assert.Equal(25, bank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(bank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(bank.Last()));
        }

        [Fact]
        public void LoadTransactions_PumbMultiPage_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2024, 04, 23, 8, 59, 44),
                Description = "FOP Kolomits Oksana Pe LVOV           UA Покупка",
                CardCurrencyAmount = -700.0,
                OperationAmount = -700.0,
                OperationCurrency = "UAH",
                Commission = 0.0,
                Balance = 0.0,
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2024, 4, 1, 12, 25, 47),
                Description = "CARD*0544 Списання",
                CardCurrencyAmount = -3750.0,
                OperationAmount = -3750.0,
                OperationCurrency = "UAH",
                Commission = 0.0,
                Balance = 0.0
            };

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "pumb_2_pages.pdf");
            IEnumerable<BankTransaction> bank = new Helpers.PumbHelper().ParseReport(path);

            Assert.Equal(37, bank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(bank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(bank.Last()));
        }

        [Fact]
        public void MoveNextCommand_Execute3Times_TransactionsImpoted()
        {
            List<Transaction> output = new ();

            List<AccountFilterModel> accounts = new ()
            {
                new () { Id = 1, Title = "Monobank", TotalAmount = 189671, CurrencyId = 2, IsActive = true },
                new () { Id = 2, Title = "Cash", TotalAmount = 10000, CurrencyId = 2, IsActive = true },
            };
            List<ProjectModel> projects = new () { new () { Id = 1, IsActive = true, Title = "My project" } };
            List<CategoryModel> categories = new () { new () { Title = "Комуналка", Id = 1 } };
            List<LocationModel> locations = new ()
            {
                new () { Id = 200, Title = "Internet", Address = "Київстар" },
                new () { Id = 201, Title = "Рошен", Address = "Roshen" },
                new () { Id = 202, Title = "Твій сир", Address = "Tvijsir" },
                new () { Id = 203, Title = "АТБ" },
                new () { Id = 204, Title = "Арсен" },
                new () { Id = 205, Title = "Сільпо" },
            };
            List<CurrencyModel> currencies = new () { new () { Id = 1, Name = "USD" }, new () { Id = 2, Name = "UAH" } };

            DbManual.SetupTests(categories);
            DbManual.SetupTests(currencies);
            DbManual.SetupTests(locations);
            DbManual.SetupTests(accounts);
            DbManual.SetupTests(projects);

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            var mono = new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM("Monobank", mono, new Dictionary<int, BlotterModel>());

            vm.RequestClose += (sender, args) => { output = sender as List<Transaction>; };

            Assert.Equal(1, ((Page1VM)vm.CurrentPage).MonoAccount.Id);
            Assert.Equal("Please select account", vm.Title);

            vm.MoveNextCommand.Execute();

            Assert.Equal("Відсотки за жовтень", ((Page2VM)vm.CurrentPage).StartTransaction.Description);
            Assert.Equal(1, ((Page2VM)vm.CurrentPage).MonoAccount.Id);
            Assert.Equal("Please select transaction", vm.Title);

            vm.MoveNextCommand.Execute();

            Assert.Equal("Please select categories", vm.Title);

            var page3 = vm.CurrentPage as Page3VM;

            // Transfer From Mono
            page3.FinancierTransactions[1].ToAccountId = 2;

            // Transfer To Mono
            page3.FinancierTransactions[40].FromAccountId = 2;

            // Expanse with project
            page3.FinancierTransactions[6].ProjectId = 1;

            vm.MoveNextCommand.Execute();

            Assert.Equal(45, output.Count);

            // Transfer From Mono
            var fromMono = output[1];
            Assert.Equal(1, fromMono.FromAccountId);
            Assert.Equal(2, fromMono.ToAccountId);
            Assert.Equal(-3000, fromMono.FromAmount);
            Assert.Equal(3000, fromMono.ToAmount);
            Assert.Equal(new DateTimeOffset(new DateTime(2021, 11, 15, 10, 18, 09)).ToUnixTimeMilliseconds(), fromMono.DateTime);

            // Transfer To Mono
            var toMono = output[40];
            Assert.Equal(2, toMono.FromAccountId);
            Assert.Equal(1, toMono.ToAccountId);
            Assert.Equal(280000, toMono.ToAmount);
            Assert.Equal(-280000, toMono.FromAmount);
            Assert.Equal(0, toMono.ProjectId);

            // Expanse with project
            var withProject = output[6];
            Assert.Equal(1, withProject.ProjectId);

            Assert.Equal(1, output.Count(x => x.CategoryId == 1));
            Assert.Equal(2, output.Count(x => x.LocationId == 200));
            Assert.Equal(3, output.Count(x => x.LocationId == 201));
            Assert.Equal(2, output.Count(x => x.LocationId == 202));
            Assert.Equal(1, output.Count(x => x.LocationId == 203));
            Assert.Equal(2, output.Count(x => x.LocationId == 204));
            Assert.Equal(5, output.Count(x => x.LocationId == 205));
            Assert.Equal(2, output.Count(x => x.OriginalCurrencyId == 1));

            DbManual.ResetAllManuals();
        }
        
        [Fact]
        public void MoveNextCommand_ParseDescription_TransactionsImpoted()
        {
            List<Transaction> output = new ();

            List<AccountFilterModel> accounts = new ()
            {
                new () { Id = 1, Title = "Monobank", TotalAmount = 189671, CurrencyId = 2, IsActive = true },
                new () { Id = 2, Title = "Cash", TotalAmount = 10000, CurrencyId = 2, IsActive = true },
                new () { Id = 3, Title = "Bank1", Number = "0544", TotalAmount = 10000, CurrencyId = 2, IsActive = true },
                new () { Id = 4, Title = "Bank2", Number = "7134", TotalAmount = 10000, CurrencyId = 2, IsActive = true },
            };
            List<ProjectModel> projects = new () { new () { Id = 1, IsActive = true, Title = "My project" } };
            List<CategoryModel> categories = new () { new () { Title = "Комуналка", Id = 1 } };
            List<CurrencyModel> currencies = new () { new () { Id = 1, Name = "USD" }, new () { Id = 2, Name = "UAH" } };

            DbManual.SetupTests(categories);
            DbManual.SetupTests(currencies);
            DbManual.SetupTests(accounts);
            DbManual.SetupTests(projects);

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.eng.transfer.csv");
            var mono = new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM("Monobank", mono, new Dictionary<int, BlotterModel>());

            vm.RequestClose += (sender, args) => { output = sender as List<Transaction>; };
            vm.MoveNextCommand.Execute();
            vm.MoveNextCommand.Execute();
            vm.MoveNextCommand.Execute();

            // Transfer From Mono
            var fromMono = output[0];
            var toMono = output[5];

            Assert.Equal(1, fromMono.FromAccountId);
            Assert.Equal(3, fromMono.ToAccountId);

            // Transfer To Mono
            Assert.Equal(4, toMono.FromAccountId);
            Assert.Equal(1, toMono.ToAccountId);


            DbManual.ResetAllManuals();
        }
    }
}
