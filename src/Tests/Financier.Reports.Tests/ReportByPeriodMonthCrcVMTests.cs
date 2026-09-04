namespace Financier.Reports.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Moq;
    using OxyPlot.Series;
    using Xunit;

    public class ReportByPeriodMonthCrcVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ReportByPeriodMonthCrcVM vm;

        public ReportByPeriodMonthCrcVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportByPeriodMonthCrcModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportByPeriodMonthCrcModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ReportByPeriodMonthCrcVM(this.dbMock.Object);
            this.vm.StartYearMonths = new YearMonths();
            this.vm.EndYearMonths = new YearMonths();
        }

        [Fact]
        public void GetPlotModel_FirstTwoSeriesAreBarSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportByPeriodMonthCrcModel>());

            Assert.IsType<BarSeries>(model.Series[0]);
            Assert.IsType<BarSeries>(model.Series[1]);
        }

        [Fact]
        public void GetPlotModel_HasOneLegend()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportByPeriodMonthCrcModel>());

            Assert.Single(model.Legends);
        }

        [Fact]
        public void GetPlotModel_HasThreeSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportByPeriodMonthCrcModel>());

            Assert.Equal(3, model.Series.Count);
        }

        [Fact]
        public void GetPlotModel_HasTwoAxes()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportByPeriodMonthCrcModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetPlotModel_ThirdSeriesIsLineSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportByPeriodMonthCrcModel>());

            Assert.IsType<LineSeries>(model.Series[2]);
        }

        [Fact]
        public void GetPlotModel_WithData_CreditSeriesHasExpectedValues()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportByPeriodMonthCrcModel>
            {
                new TestModel(year: 2024, month: 1, creditSum: 500.0, debitSum: 300.0, saldo: 200.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var creditSeries = (BarSeries)model.Series[0];

            Assert.Equal(500.0, creditSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetPlotModel_WithData_DebitSeriesHasExpectedItemCount()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportByPeriodMonthCrcModel>
            {
                new TestModel(year: 2024, month: 1, creditSum: 500.0, debitSum: 300.0, saldo: 200.0),
                new TestModel(year: 2024, month: 2, creditSum: 400.0, debitSum: 250.0, saldo: 150.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var debitSeries = (BarSeries)model.Series[1];

            Assert.Equal(2, debitSeries.ActualItems.Count);
        }

        [Fact]
        public void GetPlotModel_WithData_DebitSeriesHasExpectedValues()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportByPeriodMonthCrcModel>
            {
                new TestModel(year: 2024, month: 1, creditSum: 500.0, debitSum: 300.0, saldo: 200.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var debitSeries = (BarSeries)model.Series[1];

            Assert.Equal(300.0, debitSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetPlotModel_WithData_SaldoSeriesHasExpectedPoints()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportByPeriodMonthCrcModel>
            {
                new TestModel(year: 2024, month: 1, creditSum: 500.0, debitSum: 300.0, saldo: 200.0),
                new TestModel(year: 2024, month: 2, creditSum: 400.0, debitSum: 250.0, saldo: 150.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var saldoSeries = (LineSeries)model.Series[2];

            Assert.Equal(2, saldoSeries.Points.Count);
            Assert.Equal(200.0, saldoSeries.Points[0].Y);
            Assert.Equal(150.0, saldoSeries.Points[1].Y);
        }

        [Fact]
        public void GetSql_NoCurrencyFilter_ContainsZeroForCurrencyFlag()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.Contains("0 =", sql);
        }

        [Fact]
        public void GetSql_NoCurrencyFilter_DoesNotContainCurrencyIdClause()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.DoesNotContain("from_account_crc_id", sql);
        }

        [Fact]
        public void GetSql_ReturnsNonEmptySql()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void GetSql_WithCurrencyFilter_ContainsCurrencyIdClause()
        {
            var testVm = CreateTestableVM();
            testVm.CurentCurrency = new CurrencyModel { Id = 3 };

            var sql = testVm.TestGetSql();

            Assert.Contains("from_account_crc_id = 3", sql);
        }

        [Fact]
        public void GetSql_WithCurrencyFilter_ContainsOneForCurrencyFlag()
        {
            var testVm = CreateTestableVM();
            testVm.CurentCurrency = new CurrencyModel { Id = 3 };

            var sql = testVm.TestGetSql();

            Assert.Contains("1 =", sql);
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

            this.dbMock.Verify(x => x.ExecuteQuery<ReportByPeriodMonthCrcModel>(It.IsAny<string>()), Times.Once);
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
                .Setup(x => x.ExecuteQuery<ReportByPeriodMonthCrcModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportByPeriodMonthCrcModel>
                {
                    new TestModel(year: 2024, month: 1, creditSum: 500.0, debitSum: 300.0, saldo: 200.0),
                    new TestModel(year: 2024, month: 2, creditSum: 400.0, debitSum: 250.0, saldo: 150.0),
                });

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(2, this.vm.Entities.Count);
        }

        private TestableVM CreateTestableVM()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.StartYearMonths = new YearMonths();
            testVm.EndYearMonths = new YearMonths();
            return testVm;
        }

        private sealed class TestableVM : ReportByPeriodMonthCrcVM
        {
            public TestableVM(IFinancierDatabase db) : base(db)
            {
            }

            public SafePlotModel TestGetPlotModel(List<ReportByPeriodMonthCrcModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ReportByPeriodMonthCrcModel
        {
            public TestModel(long year, long month, double? creditSum, double? debitSum, double? saldo)
            {
                Year = year;
                Month = month;
                CreditSum = creditSum;
                DebitSum = debitSum;
                Saldo = saldo;
            }
        }
    }
}
