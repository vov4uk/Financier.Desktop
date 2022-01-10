namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.Monobank;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class MonoWizardVMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveParameters_CurrentPageNotEmpty(
            List<MonoTransaction> mono,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            var vm = new MonoWizardVM(
                mono,
                accounts,
                currencies,
                locations,
                categories,
                projects);

            Assert.NotNull(vm.CurrentPage);
        }

        [Theory]
        [AutoMoqData]
        public async Task LoadTransactions_UkrHeaders_TransactionsLoaded(
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            var mono = await new Helpers.MonoCsvHelper().ParseCsv(csvPath);
            var vm = new MonoWizardVM(
                mono,
                accounts,
                currencies,
                locations,
                categories,
                projects);

            Assert.Equal(46, ((Page2VM)vm.Pages[1]).MonoTransactions.Count);
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Theory]
        [AutoMoqData]
        public async Task LoadTransactions_EngHeaders_TransactionsLoaded(
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.eng.csv");
            var mono = await new Helpers.MonoCsvHelper().ParseCsv(csvPath);
            var vm = new MonoWizardVM(
                mono,
                accounts,
                currencies,
                locations,
                categories,
                projects);

            Assert.Single(((Page2VM)vm.Pages[1]).MonoTransactions);
            Assert.NotNull(vm.CurrentPage);
            Assert.Equal(3, vm.Pages.Count);
        }

        [Fact]
        public async Task MoveNextCommand_Execute3Times_TransactionsImpoted()
        {
            List<Transaction> output = new ();

            List<Account> accounts = new List<Account>
            {
                new Account { Id = 1, Title = "Monobank", TotalAmount = 189671, CurrencyId = 2, IsActive = true },
                new Account { Id = 2, Title = "Cash", TotalAmount = 10000, CurrencyId = 2, IsActive = true },
            };
            List<Project> projects = new List<Project> { new Project { Id = 1, IsActive = true, Title = "My project" } };
            List<Category> categories = new List<Category> { new Category { Title = "Комуналка", Id = 1 } };
            List<Location> locations = new List<Location>
            {
                new Location { Id = 200, Title = "Internet", Address = "Київстар" },
                new Location { Id = 201, Title = "Рошен", Address = "Roshen" },
                new Location { Id = 202, Title = "Твій сир", Address = "Tvijsir" },
                new Location { Id = 203, Title = "АТБ" },
                new Location { Id = 204, Title = "Арсен" },
                new Location { Id = 205, Title = "Сільпо" },
            };
            List<Currency> currencies = new List<Currency> { new Currency { Id = 1, Name = "USD" }, new Currency { Id = 2, Name = "UAH" } };

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            var mono = await new Helpers.MonoCsvHelper().ParseCsv(csvPath);
            var vm = new MonoWizardVM(
                mono,
                accounts,
                currencies,
                locations,
                categories,
                projects);

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
        }
    }
}
