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

    public class ReportDynamicRestVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ReportDynamicRestVM vm;

        public ReportDynamicRestVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportDynamicRestModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportDynamicRestModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ReportDynamicRestVM(this.dbMock.Object);
            this.vm.StartYearMonths = new YearMonths();
            this.vm.EndYearMonths = new YearMonths();
        }

        [Fact]
        public void GetPlotModel_FirstAxisIsDateTimeAxis()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicRestModel>());

            Assert.IsType<DateTimeAxis>(model.Axes[0]);
        }

        [Fact]
        public void GetPlotModel_HasOneLineSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicRestModel>());

            Assert.Single(model.Series);
            Assert.IsType<LineSeries>(model.Series[0]);
        }

        [Fact]
        public void GetPlotModel_HasTwoAxes()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicRestModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetPlotModel_NullTotal_TreatedAsZero()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicRestModel>
            {
                new TestModel(year: 2024, month: 1, day: 1, total: null),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(0.0, lineSeries.Points[0].Y);
        }

        [Fact]
        public void GetPlotModel_OrdersPointsByDay_WhenSameYearAndMonth()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicRestModel>
            {
                new TestModel(year: 2024, month: 1, day: 20, total: 200.0),
                new TestModel(year: 2024, month: 1, day: 5,  total: 500.0),
                new TestModel(year: 2024, month: 1, day: 15, total: 150.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(500.0, lineSeries.Points[0].Y);
            Assert.Equal(150.0, lineSeries.Points[1].Y);
            Assert.Equal(200.0, lineSeries.Points[2].Y);
        }

        [Fact]
        public void GetPlotModel_OrdersPointsByYearThenMonthThenDay()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicRestModel>
            {
                new TestModel(year: 2024, month: 3, day: 1,  total: 300.0),
                new TestModel(year: 2024, month: 1, day: 20, total: 100.0),
                new TestModel(year: 2024, month: 2, day: 5,  total: 200.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(100.0, lineSeries.Points[0].Y);
            Assert.Equal(200.0, lineSeries.Points[1].Y);
            Assert.Equal(300.0, lineSeries.Points[2].Y);
        }

        [Fact]
        public void GetPlotModel_SecondAxisIsLinearAxis()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportDynamicRestModel>());

            Assert.IsType<LinearAxis>(model.Axes[1]);
        }

        [Fact]
        public void GetPlotModel_WithData_LineSeriesHasExpectedPointCount()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportDynamicRestModel>
            {
                new TestModel(year: 2024, month: 1, day: 15, total: 1000.0),
                new TestModel(year: 2024, month: 2, day: 10, total: 1200.0),
                new TestModel(year: 2024, month: 3, day: 5,  total: 900.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var lineSeries = (LineSeries)model.Series[0];

            Assert.Equal(3, lineSeries.Points.Count);
        }

        [Fact]
        public void GetSql_ContainsBaseQueryText()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.Contains("ReportDynamicRestVM", sql);
        }

        [Fact]
        public void GetSql_WithNoFilters_DoesNotContainAndClause()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.DoesNotContain(" and ", sql);
        }

        [Fact]
        public void GetSql_WithNoFilters_ReturnsNonEmptySql()
        {
            var testVm = CreateTestableVM();

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

            this.dbMock.Verify(x => x.ExecuteQuery<ReportDynamicRestModel>(It.IsAny<string>()), Times.Once);
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
                .Setup(x => x.ExecuteQuery<ReportDynamicRestModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportDynamicRestModel>
                {
                    new TestModel(year: 2024, month: 1, day: 15, total: 1000.0),
                    new TestModel(year: 2024, month: 2, day: 10, total: 1200.0),
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

        private sealed class TestableVM : ReportDynamicRestVM
        {
            public TestableVM(IFinancierDatabase db)
                : base(db)
            {
            }

            public SafePlotModel TestGetPlotModel(List<ReportDynamicRestModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ReportDynamicRestModel
        {
            public TestModel(int year, int month, int day, double? total)
            {
                Year = year;
                Month = month;
                Day = day;
                Total = total;
            }
        }
    }
}
