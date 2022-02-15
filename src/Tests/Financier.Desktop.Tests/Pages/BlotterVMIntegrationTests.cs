using Financier.Common.Entities;
using Financier.DataAccess;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views;
using Financier.Tests.Common;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Financier.Desktop.Tests.Pages
{
    public class BlotterVMIntegrationTests
    {
        private readonly Mock<IDialogWrapper> dialogMock;
        private FinancierDatabase db;
        public BlotterVMIntegrationTests()
        {
            this.dialogMock = new Mock<IDialogWrapper>();
        }

        [Fact]
        public async Task EditSplitTransaction_ReplaceSubTransactionWithSubTransfer_BalancesUpdated()
        {
            await SetupDb(EditSplitTransactions());

            var resultVm = EditSplitTransactionDto();
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);
            vm.SelectedValue = new Common.Model.BlotterModel { Id = 27160, CategoryId = -1 };
            await vm.EditCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(EditSplitTransactionRunningBalancesJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(834, result.Accounts.FirstOrDefault(x => x.Id == 1).TotalAmount);
            Assert.Equal(-24230, result.Accounts.FirstOrDefault(x => x.Id == 2).TotalAmount);
            Assert.Equal(834, result.Transactions.FirstOrDefault(x => x.Id == 27173).ToAmount); // added transfer
            Assert.Null(result.Transactions.FirstOrDefault(x => x.Id == 27169)); // transaction 27169 was deleted
        }

        [Fact]
        public async Task CreateSplitTransaction_SubTransactionWithSubTransfer_BalancesUpdated()
        {
            await SetupDb();

            var resultVm = CreateSplitTransactionWithTransferTransactionDto();
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            await vm.AddCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(CreateSplitTransactionRunningBalancesJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(-100300, result.Accounts.FirstOrDefault(x => x.Id == 1).TotalAmount);
            Assert.Equal(100000, result.Accounts.FirstOrDefault(x => x.Id == 2).TotalAmount);
        }

        [Fact]
        public async Task CreateSplitTransaction_DifferentCurrencies_BalancesUpdated()
        {
            await SetupDb();

            var resultVm = CreateSplitTransactionDifferentCurrencyDto();
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            await vm.AddCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(CreateSplitTransactionDifferentCurrenciesRunningBalancesJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(-54703, result.Accounts.FirstOrDefault(x => x.Id == 2).TotalAmount);
            Assert.Equal(-54703, result.Transactions.Where(x => x.ParentId > 0).Sum(x => x.FromAmount));
            Assert.Equal(-1933, result.Transactions.Where(x => x.ParentId > 0).Sum(x => x.OriginalFromAmount));
        }

        [Theory]
        [InlineData(CreateTransferHomeCurrencyDto, CreateTransferHomeCurrencyRunningBalancesJson, -28000, 28000)]
        [InlineData(CreateTransferDiffCurrencyDto, CreateTransferDiffCurrencyRunningBalancesJson, -28000, 1000)]
        public async Task CreateTransfer_HomeCurrency_BalancesUpdated(string resultVmJson, string balanceJson, long fromAmount, long toAmount)
        {
            await SetupDb();

            var resultVm = JsonConvert.DeserializeObject<TransferDto>(resultVmJson);
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.Is<TransferControlVM>(x => x.Transfer.Id == 0), 385, 340, "Transfer"))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            await vm.AddTransferCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(balanceJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(fromAmount, result.Transactions[0].FromAmount);
            Assert.Equal(toAmount, result.Transactions[0].ToAmount);
        }

        [Theory]
        [InlineData(DuplicateTransferHomeCurrency, CreateTransferHomeCurrencyDto, DuplicateTransferHomeCurrencyRunningBalancesJson, 2, -28000, 28000)]
        [InlineData(DuplicateTransferDiffCurrency, CreateTransferDiffCurrencyDto, DuplicateTransferDiffCurrencyRunningBalancesJson, 3, -28000, 1000)]
        public async Task DuplicateTransfer_DifferentCurrency_BalancesUpdated(string transactionJson, string resultVmJson, string balanceJson, int toAccount, long fromAmount, long toAmount)
        {
            var transaction = JsonDeserializer.Deserialize<Transaction>(transactionJson);
            await SetupDb(transaction);

            var resultVm = JsonConvert.DeserializeObject<TransferDto>(resultVmJson);
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.Is<TransferControlVM>(x => x.Transfer.Id == 0), 385, 340, "Transfer"))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            vm.SelectedValue = new Common.Model.BlotterModel { Id = 1, CategoryId = 0, FromAccountId = 1, ToAccountId = toAccount };
            await vm.DuplicateCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(balanceJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(fromAmount, result.Transactions[1].FromAmount);
            Assert.Equal(toAmount, result.Transactions[1].ToAmount);
        }

        [Theory]
        [InlineData(CreateTransactionHomeCurrencyDto, CreateTransactionHomeCurrencyRunningBalancesJson, -10000, 0)]
        [InlineData(CreateTransactionDiffCurrencyDto, CreateTransactionDiffCurrencyRunningBalancesJson, -54703, -1933)]
        public async Task CreateTransaction_DifferentCurrency_BalancesUpdated(string resultVmJson, string balanceJson, long fromAmount, long originFromAmount)
        {
            await SetupDb();

            var resultVm = JsonConvert.DeserializeObject<TransactionDto>(resultVmJson);
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.Is<TransactionControlVM>(x => x.Transaction.Id == 0), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            await vm.AddCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(balanceJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(fromAmount, result.Transactions[0].FromAmount);
            Assert.Equal(originFromAmount, result.Transactions[0].OriginalFromAmount);
        }

        [Theory]
        [InlineData(DuplicateTransactionDiffCurrency, CreateTransactionDiffCurrencyDto, DuplicateTransactionDiffCurrencyRunningBalancesJson, -54703, -1933)]
        public async Task DuplicateTransaction_DifferentCurrency_BalancesUpdated(string transactionJson, string resultVmJson, string balanceJson, long fromAmount, long originFromAmount)
        {
            var transaction = JsonDeserializer.Deserialize<Transaction>(transactionJson);
            await SetupDb(transaction);

            var resultVm = JsonConvert.DeserializeObject<TransactionDto>(resultVmJson);
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.Is<TransactionControlVM>(x => x.Transaction.Id == 0), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            vm.SelectedValue = new Common.Model.BlotterModel { Id = 1, CategoryId = 37, FromAccountId = 2 };
            await vm.DuplicateCommand.ExecuteAsync();

            var result = await GetResults();
            Console.WriteLine(JsonConvert.SerializeObject(result.Balances.OrderByDescending(x => x.TransactionId)));
            Assert.Equal(2, result.Balances.Count);
            Assert.Equal(balanceJson, JsonConvert.SerializeObject(result.Balances.OrderByDescending(x => x.TransactionId)));
            Assert.Equal(fromAmount, result.Transactions[1].FromAmount);
            Assert.Equal(originFromAmount, result.Transactions[1].OriginalFromAmount);
        }

        [Theory]
        [InlineData(DuplicateTransactionHomeCurrency, CreateTransactionHomeCurrencyDto, DuplicateTransactionHomeCurrencyRunningBalancesJson, -10000, 0)]
        public async Task DuplicateTransaction_HomeCurrency_BalancesUpdated(string transactionJson, string resultVmJson, string balanceJson, long fromAmount, long originFromAmount)
        {
            var transaction = JsonDeserializer.Deserialize<Transaction>(transactionJson);
            await SetupDb(transaction);

            var resultVm = JsonConvert.DeserializeObject<TransactionDto>(resultVmJson);
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.Is<TransactionControlVM>(x => x.Transaction.Id == 0), 640, 340, nameof(Transaction)))
                .Returns(resultVm);

            var vm = new BlotterVM(db, dialogMock.Object);

            vm.SelectedValue = new Common.Model.BlotterModel { Id = 1, CategoryId = 37, FromAccountId = 2 };
            await vm.DuplicateCommand.ExecuteAsync();

            var result = await GetResults();

            Assert.Equal(2, result.Balances.Count);
            Assert.Equal(balanceJson, JsonConvert.SerializeObject(result.Balances));
            Assert.Equal(fromAmount, result.Transactions[1].FromAmount);
            Assert.Equal(originFromAmount, result.Transactions[1].OriginalFromAmount);
        }

        private async Task SetupDb(List<Transaction> transactions = null)
        {
            db = new FinancierDatabase();
            await db.SeedAsync();
            using (var uow = db.CreateUnitOfWork())
            {
                await uow.GetRepository<Currency>().AddRangeAsync(Currencies());
                await uow.GetRepository<Account>().AddRangeAsync(Accounts());
                await uow.GetRepository<Category>().AddRangeAsync(Categoires());

                if (transactions != null && transactions.Any())
                {
                    await uow.GetRepository<Transaction>().AddRangeAsync(transactions);
                }

                await uow.SaveChangesAsync();
            }
            DbManual.ResetAllManuals();
            await DbManual.SetupAsync(db);
        }

        private async Task<(List<Account> Accounts, List<RunningBalance> Balances, List<Transaction> Transactions)> GetResults()
        {
            List<Account> Accounts = new();
            List<Transaction> Transactions = new();
            List<RunningBalance> Balances = new();
            using (var uow = db.CreateUnitOfWork())
            {
                Transactions = await uow.GetRepository<Transaction>().GetAllAsync();
                Balances = await uow.GetRepository<RunningBalance>().GetAllAsync();
                Accounts = await uow.GetRepository<Account>().GetAllAsync();
            }
            return (Accounts, Balances, Transactions);
        }

        private string EditSplitTransactionRunningBalancesJson =>
"[{\"TransactionId\":27160,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-24230}," +
"{\"TransactionId\":27173,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":834}]";

        private string CreateSplitTransactionRunningBalancesJson => "[{\"TransactionId\":1,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-100300}," +
"{\"TransactionId\":3,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":100000}]";

        private const string CreateTransferDiffCurrencyRunningBalancesJson =
"[{\"TransactionId\":1,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-28000}," +
                    "{\"TransactionId\":1,\"AccountId\":3,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":1000}]";

        private const string CreateTransferHomeCurrencyRunningBalancesJson = "[{\"TransactionId\":1,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-28000}," +
"{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":28000}]";

        private const string DuplicateTransferHomeCurrencyRunningBalancesJson = "[" +
"{\"TransactionId\":2,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-56000}," +
"{\"TransactionId\":1,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-28000}," +
"{\"TransactionId\":2,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":56000}," +
"{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":28000}" +
"]";

        private const string DuplicateTransferDiffCurrencyRunningBalancesJson = "[" +
"{\"TransactionId\":2,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-56000}," +
"{\"TransactionId\":1,\"AccountId\":1,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-28000}," +
"{\"TransactionId\":2,\"AccountId\":3,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":2000}," +
"{\"TransactionId\":1,\"AccountId\":3,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":1000}" +
"]";

        private const string DuplicateTransactionHomeCurrencyRunningBalancesJson = "[" +
"{\"TransactionId\":2,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-20000}," +
"{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-10000}" +
"]";

        private const string DuplicateTransactionDiffCurrencyRunningBalancesJson = "[" +
"{\"TransactionId\":2,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-109406}," +
"{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-54703}" +
"]";

        private string CreateSplitTransactionDifferentCurrenciesRunningBalancesJson =>
 "[{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-54703}]";

        private const string CreateTransactionHomeCurrencyRunningBalancesJson =
 "[{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-10000}]";

        private const string CreateTransactionDiffCurrencyRunningBalancesJson =
 "[{\"TransactionId\":1,\"AccountId\":2,\"Account\":null,\"Transaction\":null,\"Datetime\":0,\"Balance\":-54703}]";

        private TransactionDto EditSplitTransactionDto()
        {
            var transaction = JsonConvert.DeserializeObject<TransactionDto>(
"{\"Date\":\"2022-02-13T00:00:00+02:00\",\"Time\":\"2022-02-13T11:46:48+02:00\"," +
"\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0," +
"\"Category\":{\"Id\":-1,\"Title\":\"<SPLIT_CATEGORY>\",\"Level\":0,\"Left\":0,\"Right\":0,\"Type\":0},\"CategoryId\":-1,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":true,\"UnsplitAmount\":0, " +
"\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-24230,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-24230,\"SplitAmount\":-24230,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27160,\"IsSubTransaction\":false}");

            var subTransactions = JsonConvert.DeserializeObject<List<TransactionDto>>(
"[" +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":84,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-3455,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-3455,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27165,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":37,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-3892,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-3892,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27166,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":43,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-5499,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-5499,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27167,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":68,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-3718,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-3718,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27168,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":68,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-834,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0, \"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-834, \"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27170,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":43,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-3269,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-3269,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27171,\"IsSubTransaction\":false}," +
"{\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":37,\"OriginalCurrency\":null,\"OriginalCurrencyId\":0,\"IsSplitCategory\":false,\"UnsplitAmount\":0,\"SubTransactions\":[],\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsAmountNegative\":true,\"FromAmount\":-2729,\"IsOriginalFromAmountVisible\":false,\"OriginalFromAmount\":0,\"Rate\":0.0,\"RateString\":\"N/A\",\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-2729,\"SplitAmount\":0,\"DateTime\":\"2022-02-13T11:46:48\",\"Id\":27172,\"IsSubTransaction\":false}" +
"]");

            var transfer = JsonConvert.DeserializeObject<TransferDto>(
"{\"IsSubTransaction\":false, \"FromAccountId\":2, \"ToAccountId\":1, \"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Mono\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968}, " +
"\"ToAccount\":  {\"Id\":1,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":233000}, " +
"\"FromAmount\":-834,\"IsToAmountVisible\":false, \"ToAmount\":0, \"Rate\":0.0, \"RateString\":\"N/A\", \"Note\":null, \"IsAmountNegative\":true, \"RealFromAmount\":-834, \"DateTime\":\"2022-02-13T11:46:48\", \"Id\":0}");
            transaction.SubTransactions.Clear();
            foreach (var item in subTransactions)
            {
                transaction.SubTransactions.Add(item);
            }
            transaction.SubTransactions.Add(transfer);
            return transaction;
        }

        private TransactionDto CreateSplitTransactionDifferentCurrencyDto()
        {
            var transaction = JsonConvert.DeserializeObject<TransactionDto>(
"{\"Date\":\"2022-02-06T00:00:00+02:00\",\"Time\":\"2022-02-06T16:22:17+02:00\"," +
"\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Мій Monobank\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968}," +
"\"FromAccountId\":2,\"PayeeId\":0,\"Category\":{\"Id\":-1,\"Title\":\"<SPLIT_CATEGORY>\",\"Level\":0,\"Left\":0,\"Right\":0,\"Type\":0},\"CategoryId\":-1," +
"\"OriginalCurrency\":{\"Id\":2,\"Name\":\"USD\",\"Title\":\"United States dollar\",\"Symbol\":\"$\",\"IsDefault\":false}," +
"\"OriginalCurrencyId\":2,\"IsSplitCategory\":true,\"UnsplitAmount\":0,\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsOriginalFromAmountVisible\":true,\"IsAmountNegative\":true,\"OriginalFromAmount\":-1933," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"FromAmount\":-54703,\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-1933,\"SplitAmount\":-1933,\"DateTime\":\"2022-02-06T16:22:17\",\"Id\":26990,\"IsSubTransaction\":false}");

            var subTransactions = JsonConvert.DeserializeObject<List<TransactionDto>>(
"[" +
"{\"Date\":\"0001-01-01T00:00:00\",\"Time\":\"0001-01-01T00:00:00\",\"FromAccount\":null,\"FromAccountId\":0,\"PayeeId\":null,\"CategoryId\":37,\"OriginalCurrencyId\":null,\"IsSplitCategory\":false,\"UnsplitAmount\":-933, \"SubTransactions\":[],\"LocationId\":null,\"Note\":null,\"ProjectId\":null,\"IsOriginalFromAmountVisible\":false,\"IsAmountNegative\":true,\"OriginalFromAmount\":null,\"FromAccountCurrency\":null,\"FromAmount\":-933, \"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-933, \"SplitAmount\":0,\"DateTime\":\"0001-01-01T00:00:00\",\"Id\":0,\"IsSubTransaction\":false}," +
"{\"Date\":\"0001-01-01T00:00:00\",\"Time\":\"0001-01-01T00:00:00\",\"FromAccount\":null,\"FromAccountId\":0,\"PayeeId\":null,\"CategoryId\":43,\"OriginalCurrencyId\":null,\"IsSplitCategory\":false,\"UnsplitAmount\":-1000,\"SubTransactions\":[],\"LocationId\":null,\"Note\":null,\"ProjectId\":null,\"IsOriginalFromAmountVisible\":false,\"IsAmountNegative\":true,\"OriginalFromAmount\":null,\"FromAccountCurrency\":null,\"FromAmount\":-1000,\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-1000,\"SplitAmount\":0,\"DateTime\":\"0001-01-01T00:00:00\",\"Id\":0,\"IsSubTransaction\":false}" +
"]");

            transaction.SubTransactions.Clear();
            foreach (var item in subTransactions)
            {
                transaction.SubTransactions.Add(item);
            }
            return transaction;
        }

        private const string CreateTransferDiffCurrencyDto =
"{\"IsSubTransaction\":false,\"Date\":\"2022-02-15T00:00:00+02:00\",\"Time\":\"2022-02-15T22:05:24.722+02:00\"," +
"\"FromAccount\":{\"Id\":1,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":0},\"FromAccountId\":1," +
"\"ToAccount\":{\"Id\":3,\"IsActive\":false,\"SortOrder\":3,\"Title\":\"USD\",\"Type\":\"CASH\",\"CurrencyName\":\"USD\",\"CurrencyId\":2,\"TotalAmount\":0},\"ToAccountId\":3," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true},\"FromAmount\":28000,\"IsToAmountVisible\":true," +
"\"ToAccountCurrency\":{\"Id\":2,\"Name\":\"USD\",\"Title\":\"United States dollar\",\"Symbol\":\"$\",\"IsDefault\":false},\"ToAmount\":1000,\"Note\":null,\"RealFromAmount\":-28000,\"IsAmountNegative\":true,\"DateTime\":\"2022-02-15T22:05:24\",\"Id\":0}";

        private const string CreateTransferHomeCurrencyDto =
"{\"IsSubTransaction\":false,\"Date\":\"2022-02-15T00:00:00+02:00\",\"Time\":\"2022-02-15T22:05:24.722+02:00\"," +
"\"FromAccount\":{\"Id\":1,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":0},\"FromAccountId\":1," +
"\"ToAccount\":{\"Id\":2,\"IsActive\":false,\"SortOrder\":2,\"Title\":\"Мій Монобанк\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":0},\"ToAccountId\":2," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true},\"FromAmount\":28000,\"IsToAmountVisible\":true," +
"\"ToAccountCurrency\":  {\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true},\"ToAmount\":28000,\"Note\":null,\"RealFromAmount\":-28000,\"IsAmountNegative\":true,\"DateTime\":\"2022-02-15T22:05:24\",\"Id\":0}";

        private const string CreateTransactionHomeCurrencyDto =
"{\"Date\":\"2022-02-15T00:00:00+02:00\",\"Time\":\"2022-02-15T23:05:49.208+02:00\"," +
"\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":0}," +
"\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":37,\"OriginalCurrencyId\":null,\"IsSplitCategory\":false,\"UnsplitAmount\":-10000,\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsOriginalFromAmountVisible\":false,\"IsAmountNegative\":true,\"OriginalFromAmount\":null," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"FromAmount\":10000,\"Rate\":0.0,\"RealFromAmount\":-10000,\"SplitAmount\":0,\"DateTime\":\"2022-02-15T23:05:49\",\"Id\":0,\"IsSubTransaction\":false}";

        private const string CreateTransactionDiffCurrencyDto =
"{\"Date\":\"2022-02-06T00:00:00+02:00\",\"Time\":\"2022-02-06T16:22:17+02:00\"," +
"\"FromAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Мій Monobank\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":0}," +
"\"FromAccountId\":2,\"PayeeId\":0,\"CategoryId\":37," +
"\"OriginalCurrency\":{\"Id\":2,\"Name\":\"USD\",\"Title\":\"United States dollar\",\"Symbol\":\"$\",\"IsDefault\":false}," +
"\"OriginalCurrencyId\":2,\"IsSplitCategory\":false,\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsOriginalFromAmountVisible\":true,\"IsAmountNegative\":true,\"OriginalFromAmount\":-1933," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"FromAmount\":-54703,\"SplitAmount\":0,\"DateTime\":\"2022-02-06T16:22:17\",\"Id\":0,\"IsSubTransaction\":false}";

        private TransactionDto CreateSplitTransactionWithTransferTransactionDto()
        {
            var transaction = JsonConvert.DeserializeObject<TransactionDto>(
"{\"Date\":\"2022-02-15T00:00:00+02:00\",\"Time\":\"2022-02-15T17:47:19.515+02:00\"," +
"\"FromAccount\":{\"Id\":1,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":233000}," +
"\"FromAccountId\":1,\"PayeeId\":0,\"CategoryId\":-1,\"OriginalCurrencyId\":null,\"IsSplitCategory\":true,\"UnsplitAmount\":0,\"LocationId\":0,\"Note\":null,\"ProjectId\":0,\"IsOriginalFromAmountVisible\":false,\"IsAmountNegative\":true,\"OriginalFromAmount\":null,\"FromAmount\":100300,\"Rate\":0.0,\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-100300,\"SplitAmount\":-100300,\"DateTime\":\"2022-02-15T17:47:19\",\"Id\":0,\"IsSubTransaction\":false}");

            var subTransaction = JsonConvert.DeserializeObject<TransactionDto>(
"{\"Date\":\"0001-01-01T00:00:00\",\"Time\":\"0001-01-01T00:00:00\",\"FromAccount\":null,\"FromAccountId\":0,\"PayeeId\":null,\"CategoryId\":76,\"OriginalCurrencyId\":null,\"IsSplitCategory\":false,\"UnsplitAmount\":-300,\"SubTransactions\":[],\"LocationId\":null,\"Note\":null,\"ProjectId\":null,\"IsOriginalFromAmountVisible\":false,\"IsAmountNegative\":true,\"OriginalFromAmount\":null,\"FromAccountCurrency\":null,\"FromAmount\":-300,\"Rate\":0.0,\"ParentTransactionUnSplitAmount\":0,\"RealFromAmount\":-300,\"SplitAmount\":0,\"DateTime\":\"0001-01-01T00:00:00\",\"Id\":0,\"IsSubTransaction\":false}");

            var transfer = JsonConvert.DeserializeObject<TransferDto>(
"{\"IsSubTransaction\":false,\"Date\":\"2022-02-15T00:00:00\",\"Time\":\"2022-02-15T17:47:19\",\"FromAccountId\":1,\"ToAccountId\":2," +
"\"FromAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"FromAmount\":-100000,\"IsToAmountVisible\":false," +
"\"FromAccount\":{\"Id\":1,\"IsActive\":true,\"SortOrder\":1,\"Title\":\"Мій Гаманець\",\"Type\":\"CASH\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":233000},"+
"\"ToAccount\":{\"Id\":2,\"IsActive\":true,\"SortOrder\":2,\"Title\":\"Мій Monobank\",\"Type\":\"DEBIT_CARD\",\"CurrencyName\":\"UAH\",\"CurrencyId\":1,\"TotalAmount\":1847968},"+
"\"ToAccountCurrency\":{\"Id\":1,\"Name\":\"UAH\",\"Title\":\"Ukrainian hryvnia\",\"Symbol\":\"₴\",\"IsDefault\":true}," +
"\"ToAmount\":100000,\"Rate\":1.0,\"Note\":null,\"IsAmountNegative\":true,\"RealFromAmount\":-100000,\"DateTime\":\"2022-02-15T17:47:19\",\"Id\":0}");
            transaction.SubTransactions.Clear();
            transaction.SubTransactions.Add(subTransaction);
            transaction.SubTransactions.Add(transfer);
            return transaction;
        }

        private List<Currency> Currencies()
        {
            return JsonDeserializer.Deserialize<Currency>(
"[" +
"{\"_id\":1,\"name\":\"UAH\",\"title\":\"Ukrainian hryvnia\",\"symbol\":\"₴\",\"is_default\":1,\"decimals\":2,\"decimal_separator\":\"'.'\",\"group_separator\":\"' '\",\"symbol_format\":\"RS\",\"updated_on\":0,\"remote_key\":null,\"sort_order\":0,\"is_active\":1}," +
"{\"_id\":2,\"name\":\"USD\",\"title\":\"United States dollar\",\"symbol\":\"$\",\"is_default\":0,\"decimals\":2,\"decimal_separator\":\"'.'\",\"group_separator\":\"' '\",\"symbol_format\":\"RS\",\"updated_on\":0,\"remote_key\":null,\"sort_order\":0,\"is_active\":1}," +
"]");
        }

        private List<Account> Accounts()
        {
            return JsonDeserializer.Deserialize<Account>(
"[" +
"{\"_id\":1,\"title\":\"Мій Гаманець\",\"creation_date\":1640014165164,\"currency_id\":1,\"total_amount\":0,\"type\":\"CASH\",\"issuer\":null,\"number\":null,\"sort_order\":1,\"is_active\":1,\"is_include_into_totals\":1,\"last_category_id\":0,\"last_account_id\":12,\"total_limit\":0,\"card_issuer\":null,\"closing_day\":0,\"payment_day\":0,\"note\":null,\"last_transaction_date\":1644826445000,\"updated_on\":0,\"remote_key\":null}," +
"{\"_id\":2,\"title\":\"Мій Monobank\",\"creation_date\":1640014146783,\"currency_id\":1,\"total_amount\":0,\"type\":\"BANK\",\"issuer\":\"Monobank\",\"number\":null,\"sort_order\":2,\"is_active\":0,\"is_include_into_totals\":1,\"last_category_id\":0,\"last_account_id\":8,\"total_limit\":0,\"card_issuer\":null,\"closing_day\":0,\"payment_day\":0,\"note\":null,\"last_transaction_date\":1468056119365,\"updated_on\":0,\"remote_key\":null}," +
"{\"_id\":3,\"title\":\"USD\",         \"creation_date\":1640013732451,\"currency_id\":2,\"total_amount\":0,\"type\":\"CASH\",\"issuer\":null,\"number\":null,\"sort_order\":3,\"is_active\":0,\"is_include_into_totals\":1,\"last_category_id\":0,\"last_account_id\":25,\"total_limit\":0,\"card_issuer\":null,\"closing_day\":0,\"payment_day\":0,\"note\":null,\"last_transaction_date\":1643471070411,\"updated_on\":0,\"remote_key\":null}" +
"]");
        }

        private List<Category> Categoires()
        {
            return JsonDeserializer.Deserialize<Category>(
"[" +
"{\"_id\":37,\"title\":\"Продукти🍌\",  \"left\":25,  \"right\":26, \"last_location_id\":0,\"last_project_id\":0,\"sort_order\":0,\"type\":0,\"updated_on\":0,\"remote_key\":null,\"is_active\":1}," +
"{\"_id\":43,\"title\":\"Молочні🐮\",   \"left\":9,   \"right\":14, \"last_location_id\":0,\"last_project_id\":0,\"sort_order\":0,\"type\":0,\"updated_on\":0,\"remote_key\":null,\"is_active\":1}," +
"{\"_id\":68,\"title\":\"Солодощі🍫\",   \"left\":27,  \"right\":28, \"last_location_id\":0,\"last_project_id\":0,\"sort_order\":0,\"type\":0,\"updated_on\":0,\"remote_key\":null,\"is_active\":1}," +
"{\"_id\":76,\"title\":\"Комісія 📦\",  \"left\":162, \"right\":163,\"last_location_id\":0,\"last_project_id\":0,\"sort_order\":0,\"type\":0,\"updated_on\":0,\"remote_key\":null,\"is_active\":1},"+
"{\"_id\":84,\"title\":\"Фрукти🍇\",    \"left\":22,  \"right\":23, \"last_location_id\":0,\"last_project_id\":0,\"sort_order\":0,\"type\":0,\"updated_on\":0,\"remote_key\":null,\"is_active\":1}" +
"]");
        }

        private List<Transaction> EditSplitTransactions()
        {
            return JsonDeserializer.Deserialize<Transaction>(
"[" +
"{\"_id\":27160,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":-1,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-24230,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044154,\"payee_id\":0,\"parent_id\":0,\"updated_on\":1644825044156,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27165,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":84,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-3455,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044156,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27166,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":37,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-3892,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27167,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":43,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-5499,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27168,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":68,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-3718,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27169,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":68,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-834,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27170,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":68,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-834,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27171,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":43,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-3269,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}," +
"{\"_id\":27172,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":37,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-2729,\"to_amount\":0,\"datetime\":1644745608000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1644825044156,\"payee_id\":0,\"parent_id\":27160,\"updated_on\":1644825044157,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}" +
"]");
        }

        private const string DuplicateTransferHomeCurrency =
"[{\"_id\":1,\"from_account_id\":1,\"to_account_id\":2,\"category_id\":0,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-28000,\"to_amount\":28000,\"datetime\":1446967346062,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1447247991064,\"payee_id\":0,\"parent_id\":0,\"updated_on\":0,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}]";

        private const string DuplicateTransferDiffCurrency =
"[{\"_id\":1,\"from_account_id\":1,\"to_account_id\":3,\"category_id\":0,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-28000,\"to_amount\":1000,\"datetime\":1446967346062,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":1447247991064,\"payee_id\":0,\"parent_id\":0,\"updated_on\":0,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}]";

        private const string DuplicateTransactionDiffCurrency =
"[{\"_id\":1,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":37,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-54703,\"to_amount\":0,\"datetime\":1644157337000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":0,\"payee_id\":0,\"parent_id\":0,\"updated_on\":0,\"remote_key\":null,\"original_currency_id\":2,\"original_from_amount\":-1933,\"blob_key\":null}]";

        private const string DuplicateTransactionHomeCurrency =
"[{\"_id\":1,\"from_account_id\":2,\"to_account_id\":0,\"category_id\":37,\"project_id\":0,\"location_id\":0,\"note\":null,\"from_amount\":-10000,\"to_amount\":0,\"datetime\":1644060576000,\"provider\":null,\"accuracy\":0.0,\"latitude\":0.0,\"longitude\":0.0,\"payee\":null,\"is_template\":0,\"template_name\":null,\"recurrence\":null,\"notification_options\":null,\"status\":\"UR\",\"attached_picture\":null,\"is_ccard_payment\":0,\"last_recurrence\":0,\"payee_id\":0,\"parent_id\":0,\"updated_on\":0,\"remote_key\":null,\"original_currency_id\":0,\"original_from_amount\":0,\"blob_key\":null}]";

    }
}
