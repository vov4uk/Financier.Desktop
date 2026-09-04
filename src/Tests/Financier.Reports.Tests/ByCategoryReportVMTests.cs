namespace Financier.Reports.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Financier.Reports.Structure;
    using Moq;
    using OxyPlot.Series;
    using Xunit;

    public class ByCategoryReportVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ByCategoryReportVM vm;

        public ByCategoryReportVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ByCategoryReportModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ByCategoryReportModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ByCategoryReportVM(this.dbMock.Object);
        }

        [Fact]
        public void GetPlotModel_ExpenseItems_AddedToSecondSeries()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var items = new List<ByCategoryReportModel>
            {
                new TestModel("Groceries", isExpense: 1, parentId: 1, total: 200.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var expenseSeries = (BarSeries)model.Series[1];

            Assert.Single(expenseSeries.Items);
            Assert.Equal(200.0, expenseSeries.Items[0].Value);
        }

        [Fact]
        public void GetPlotModel_IncomeItems_AddedToFirstSeries()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var items = new List<ByCategoryReportModel>
            {
                new TestModel("Salary", isExpense: 0, parentId: 1, total: 500.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var incomeSeries = (BarSeries)model.Series[0];

            Assert.Single(incomeSeries.Items);
            Assert.Equal(500.0, incomeSeries.Items[0].Value);
        }

        [Fact]
        public void GetPlotModel_PieChart_GroupsByParentId()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var items = new List<ByCategoryReportModel>
            {
                new TestModel("Food",     isExpense: 1, parentId: 1, total: 100.0),
                new TestModel("Food",     isExpense: 1, parentId: 1, total: 50.0),
                new TestModel("Transport", isExpense: 1, parentId: 2, total: 80.0),
            };

            testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)testVm.PieChartModel.Series[0];

            Assert.Equal(2, pieSeries.Slices.Count);
        }

        [Fact]
        public void GetPlotModel_PieChart_HasOnePieSeries()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            testVm.TestGetPlotModel(new List<ByCategoryReportModel>());

            Assert.Single(testVm.PieChartModel.Series);
            Assert.IsType<PieSeries>(testVm.PieChartModel.Series[0]);
        }

        [Fact]
        public void GetPlotModel_PieChart_SliceValueIsAbsoluteGroupTotal()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var items = new List<ByCategoryReportModel>
            {
                new TestModel("Food", isExpense: 1, parentId: 1, total: -100.0),
                new TestModel("Food", isExpense: 1, parentId: 1, total: -50.0),
            };

            testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)testVm.PieChartModel.Series[0];

            Assert.Equal(150.0, pieSeries.Slices[0].Value);
        }

        [Fact]
        public void GetPlotModel_ReturnsBarChartWithOneLegend()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var model = testVm.TestGetPlotModel(new List<ByCategoryReportModel>());

            Assert.Single(model.Legends);
        }

        [Fact]
        public void GetPlotModel_ReturnsBarChartWithTwoAxes()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var model = testVm.TestGetPlotModel(new List<ByCategoryReportModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetPlotModel_ReturnsBarChartWithTwoSeries()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var model = testVm.TestGetPlotModel(new List<ByCategoryReportModel>());

            Assert.Equal(2, model.Series.Count);
            Assert.IsType<BarSeries>(model.Series[0]);
            Assert.IsType<BarSeries>(model.Series[1]);
        }

        [Fact]
        public void GetPlotModel_SetsPieChartModel()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            testVm.TestGetPlotModel(new List<ByCategoryReportModel>());

            Assert.NotNull(testVm.PieChartModel);
        }

        [Fact]
        public void GetPlotModel_UsesAbsoluteValueForBarItems()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var items = new List<ByCategoryReportModel>
            {
                new TestModel("Rent", isExpense: 1, parentId: 1, total: -300.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var expenseSeries = (BarSeries)model.Series[1];

            Assert.Equal(300.0, expenseSeries.Items[0].Value);
        }

        [Fact]
        public void GetSql_ContainsDateBetweenClause()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var sql = testVm.TestGetSql();

            Assert.Contains("BETWEEN", sql);
        }

        [Fact]
        public void GetSql_NoTopCategory_ContainsTopLevelFilter()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var sql = testVm.TestGetSql();

            Assert.Contains("parent_level = 0", sql);
        }

        [Fact]
        public void GetSql_WithFromDate_ContainsFromTimestamp()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            var fromDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
            testVm.From = fromDate;

            var sql = testVm.TestGetSql();

            Assert.NotEmpty(sql);
            Assert.Contains("BETWEEN", sql);
        }

        [Fact]
        public void GetSql_WithTopCategory_ContainsLeftRightFilter()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.TopCategory = new CategoryModel { Id = 5, Left = 10, Right = 20 };

            var sql = testVm.TestGetSql();

            Assert.Contains("parent_left > 10", sql);
            Assert.Contains("parent_right < 20", sql);
            Assert.Contains("parent_level = 1", sql);
        }

        [Fact]
        public async Task PieChartModel_RaisesPropertyChanged_AfterRefresh()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ByCategoryReportVM.PieChartModel))
                {
                    raised = true;
                }
            };

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.True(raised);
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

            this.dbMock.Verify(x => x.ExecuteQuery<ByCategoryReportModel>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RefreshDataCommand_SetsPieChartModel_AfterRefresh()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.NotNull(this.vm.PieChartModel);
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
                .Setup(x => x.ExecuteQuery<ByCategoryReportModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ByCategoryReportModel>
                {
                    new TestModel("Food",      isExpense: 1, parentId: 1, total: -100.0),
                    new TestModel("Salary",    isExpense: 0, parentId: 2, total:  200.0),
                });

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(2, this.vm.Entities.Count);
        }

        private sealed class TestableVM : ByCategoryReportVM
        {
            public TestableVM(IFinancierDatabase db)
                : base(db)
            {
            }

            public SafePlotModel TestGetPlotModel(List<ByCategoryReportModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ByCategoryReportModel
        {
            public TestModel(string category, long isExpense, long parentId, double total)
            {
                Category = category;
                IsExpense = isExpense;
                ParentId = parentId;
                Total = total;
            }
        }
    }
}
