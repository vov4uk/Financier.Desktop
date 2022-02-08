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
            var vm = new MonoWizardVM(mono);

            Assert.NotNull(vm.CurrentPage);
        }

        [Fact]
        public async Task LoadTransactions_UkrHeaders_TransactionsLoaded()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            var mono = await new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM(mono);

            Assert.Equal(46, mono.Count());
            Assert.Equal(46, ((Page2VM)vm.Pages[1]).GetMonoTransactions().Count);
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Fact]
        public async Task LoadTransactions_EngHeaders_TransactionsLoaded()
        {
            DbManual.SetupTests(new List<AccountFilterModel>());
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.eng.csv");
            IEnumerable<BankTransaction> mono = await new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM(mono);

            Assert.Single(((Page2VM)vm.Pages[1]).GetMonoTransactions());
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Fact]
        public async Task LoadTransactions_Monobank_ExpectedTransactions()
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
            IEnumerable<BankTransaction> mono = await new Helpers.MonobankHelper().ParseReport(csvPath);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(mono.ToList()));
        }

        [Fact]
        public async Task LoadTransactions_Monobank_EmptyList()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", Guid.NewGuid().ToString());
            IEnumerable<BankTransaction> mono = await new Helpers.MonobankHelper().ParseReport(csvPath);

            Assert.Empty(mono);
        }

        [Fact]
        public async Task LoadTransactions_Abank_ExpectedTransactions()
        {
            var first = new BankTransaction
            {
                Date = new DateTime(2022, 1, 19, 15, 55, 0, DateTimeKind.Local),
                Description = "McDonald’s",
                Balance = 10360.00,
                MCC = "5814",
                Commission = 0.0,
                CardCurrencyAmount = -119.00,
                OperationAmount = -119.00,
                OperationCurrency = "UAH",
                Cashback = 3.57,
            };

            var last = new BankTransaction
            {
                Date = new DateTime(2022, 1, 1, 0, 1, 0, DateTimeKind.Local),
                Description = "Відсотки на залишок власних коштів",
                Balance = 10315.12,
                Commission = 0.0,
                MCC = "4829",
                CardCurrencyAmount = 3.12,
                OperationAmount = 3.12,
                OperationCurrency = "UAH",
                Cashback = 0.0,
            };

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "abank.pdf");
            IEnumerable<BankTransaction> abank = await new Helpers.ABankHelper().ParseReport(csvPath);

            Assert.Equal(8, abank.Count());
            Assert.Equal(JsonConvert.SerializeObject(first), JsonConvert.SerializeObject(abank.First()));
            Assert.Equal(JsonConvert.SerializeObject(last), JsonConvert.SerializeObject(abank.Last()));
        }

        [Fact]
        public async Task MoveNextCommand_Execute3Times_TransactionsImpoted()
        {
            List<Transaction> output = new ();

            List<AccountFilterModel> accounts = new ()
            {
                new () { Id = 1, Title = "Monobank", TotalAmount = 189671, CurrencyId = 2, Is_Active = 1 },
                new () { Id = 2, Title = "Cash", TotalAmount = 10000, CurrencyId = 2, Is_Active = 1 },
            };
            List<ProjectModel> projects = new () { new () { Id = 1, Is_Active = 1, Title = "My project" } };
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
            var mono = await new Helpers.MonobankHelper().ParseReport(csvPath);
            var vm = new MonoWizardVM(mono);

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
    }
}
