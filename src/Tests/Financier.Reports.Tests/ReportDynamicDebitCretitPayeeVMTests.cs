namespace Financier.Reports.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Moq;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using Xunit;

    public class ReportDynamicDebitCretitPayeeVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly Mock<IDialogService> dialogMock;
        private readonly ReportDynamicDebitCretitPayeeVM vm;

        public ReportDynamicDebitCretitPayeeVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportDynamicDebitCretitPayeeModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportDynamicDebitCretitPayeeModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.dialogMock = new Mock<IDialogService>();

            this.vm = new ReportDynamicDebitCretitPayeeVM(this.dbMock.Object);
            this.vm.DialogService = this.dialogMock.Object;
            this.vm.StartYearMonths = new YearMonths();
            this.vm.EndYearMonths = new YearMonths();

            // Ensure a Category is selected so GetSql() does not reach the dialog path
            this.vm.Category = new CategoryModel { Id = 1 };
        }

        [Fact]
        public void GetPlotModel_FirstAxisIsLinearAxis()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicDebitCretitPayeeModel>());

            Assert.IsType<LinearAxis>(model.Axes[0]);
        }

        [Fact]
        public void GetPlotModel_HasOneLineSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicDebitCretitPayeeModel>());

            Assert.Single(model.Series);
            Assert.IsType<LineSeries>(model.Series[0]);
        }

        [Fact]
        public void GetPlotModel_HasTwoAxes()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicDebitCretitPayeeModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetPlotModel_NullTotal_TreatedAsZero()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicDebitCretitPayeeModel>
            {
                new TestModel(year: 2024, month: 1, total: null),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(0.0, lineSeries.Points[0].Y);
        }

        [Fact]
        public void GetPlotModel_OrdersPointsByYearThenMonth()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicDebitCretitPayeeModel>
            {
                new TestModel(year: 2024, month: 3, total: -300.0),
                new TestModel(year: 2024, month: 1, total: -100.0),
                new TestModel(year: 2024, month: 2, total: -200.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(-100.0, lineSeries.Points[0].Y);
            Assert.Equal(-200.0, lineSeries.Points[1].Y);
            Assert.Equal(-300.0, lineSeries.Points[2].Y);
        }

        [Fact]
        public void GetPlotModel_SecondAxisIsDateTimeAxis()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicDebitCretitPayeeModel>());

            Assert.IsType<DateTimeAxis>(model.Axes[1]);
        }

        [Fact]
        public void GetPlotModel_WithData_LineSeriesHasExpectedPointCount()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicDebitCretitPayeeModel>
            {
                new TestModel(year: 2024, month: 1, total: -200.0),
                new TestModel(year: 2024, month: 2, total: -150.0),
                new TestModel(year: 2024, month: 3, total: -100.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(3, lineSeries.Points.Count);
        }

        [Fact]
        public void GetSql_NeitherPayeeNorCategory_ReturnsEmpty()
        {
            var testVm = CreateTestableVM();

            // Leave both Payee.Id and Category.Id as null
            var sql = testVm.TestGetSql();

            Assert.Empty(sql);
            this.dialogMock.Verify(x => x.ShowMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetSql_NoCurrencySelected_ContainsZeroForCurrencyFlag()
        {
            var testVm = CreateTestableVM();
            testVm.Category = new CategoryModel { Id = 1 };

            var sql = testVm.TestGetSql();

            Assert.Contains("0 =", sql);
        }

        [Fact]
        public void GetSql_NoCurrencySelected_DoesNotContainCurrencyIdClause()
        {
            var testVm = CreateTestableVM();
            testVm.Category = new CategoryModel { Id = 1 };

            var sql = testVm.TestGetSql();

            Assert.DoesNotContain("from_account_crc_id", sql);
        }

        [Fact]
        public void GetSql_WithCategorySelected_ReturnsNonEmptySql()
        {
            var testVm = CreateTestableVM();
            testVm.Category = new CategoryModel { Id = 1 };

            var sql = testVm.TestGetSql();

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void GetSql_WithCurrencySelected_ContainsCurrencyIdClause()
        {
            var testVm = CreateTestableVM();
            testVm.Category = new CategoryModel { Id = 1 };
            testVm.CurentCurrency = new CurrencyModel { Id = 7 };

            var sql = testVm.TestGetSql();

            Assert.Contains("from_account_crc_id = 7", sql);
        }

        [Fact]
        public void GetSql_WithCurrencySelected_ContainsOneForCurrencyFlag()
        {
            var testVm = CreateTestableVM();
            testVm.Category = new CategoryModel { Id = 1 };
            testVm.CurentCurrency = new CurrencyModel { Id = 7 };

            var sql = testVm.TestGetSql();

            Assert.Contains("1 =", sql);
        }

        [Fact]
        public void GetSql_WithPayeeSelected_ReturnsNonEmptySql()
        {
            var testVm = CreateTestableVM();
            testVm.Payee = new PayeeModel { Id = 5 };

            var sql = testVm.TestGetSql();

            Assert.NotEmpty(sql);
        }

        [Fact]
        public async Task RefreshDataCommand_EmptyData_EntitiesIsEmpty()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Empty(this.vm.Entities);
        }

        [Fact]
        public async Task RefreshDataCommand_ExecutesQueryOnce()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.ExecuteQuery<ReportDynamicDebitCretitPayeeModel>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RefreshDataCommand_SetsPlotModel_AfterRefresh()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.NotNull(this.vm.PlotModel);
        }

        [Fact]
        public async Task RefreshDataCommand_WithData_PopulatesEntities()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportDynamicDebitCretitPayeeModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportDynamicDebitCretitPayeeModel>
                {
                    new TestModel(year: 2024, month: 1, total: -200.0),
                    new TestModel(year: 2024, month: 2, total: -150.0),
                });

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(2, this.vm.Entities.Count);
        }

        private TestableVM CreateTestableVM()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.DialogService = this.dialogMock.Object;
            testVm.StartYearMonths = new YearMonths();
            testVm.EndYearMonths = new YearMonths();
            return testVm;
        }

        private sealed class TestableVM : ReportDynamicDebitCretitPayeeVM
        {
            public TestableVM(IFinancierDatabase db) : base(db)
            {
            }

            public SafePlotModel TestGetPlotModel(List<ReportDynamicDebitCretitPayeeModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ReportDynamicDebitCretitPayeeModel
        {
            public TestModel(int year, int month, double? total)
            {
                Year = year;
                Month = month;
                Total = total;
            }
        }
    }
}
