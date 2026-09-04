namespace Financier.Reports.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Moq;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using Xunit;

    public class ReportStructureIncomeExpenseVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ReportStructureIncomeExpenseVM vm;

        public ReportStructureIncomeExpenseVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureIncomeExpenseModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureIncomeExpenseModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ReportStructureIncomeExpenseVM(this.dbMock.Object);
            this.vm.StartYearMonths = new YearMonths();
            this.vm.EndYearMonths = new YearMonths();
        }

        [Fact]
        public void Constructor_SetsDefaultIsIncome_ToFalse()
        {
            Assert.False(this.vm.IsIncome);
        }

        [Fact]
        public void GetBarChartModel_HasOneLegend()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.Single(model.Legends);
        }

        [Fact]
        public void GetBarChartModel_IsIncome_FillColorIsGreen()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = true;

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal(OxyColors.Green, barSeries.FillColor);
        }

        [Fact]
        public void GetBarChartModel_IsIncome_LabelFormatStringIsPositive()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = true;

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal("{0}", barSeries.LabelFormatString);
        }

        [Fact]
        public void GetBarChartModel_NotIncome_FillColorIsOrange()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = false;

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal(OxyColors.Orange, barSeries.FillColor);
        }

        [Fact]
        public void GetBarChartModel_NotIncome_LabelFormatStringHasNegativeSign()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = false;

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal("-{0}", barSeries.LabelFormatString);
        }

        [Fact]
        public void GetBarChartModel_OrdersItemsByAbsoluteTotalAscending()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureIncomeExpenseModel>
            {
                new TestModel("Big", -300.0),
                new TestModel("Small", -50.0),
                new TestModel("Medium", -150.0),
            };

            var model = testVm.TestGetBarChartModel(items);
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal(50, barSeries.ActualItems[0].Value);
            Assert.Equal(150, barSeries.ActualItems[1].Value);
            Assert.Equal(300, barSeries.ActualItems[2].Value);
        }

        [Fact]
        public void GetBarChartModel_ReturnsModelWithOneBarSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.Single(model.Series);
            Assert.IsType<BarSeries>(model.Series[0]);
        }

        [Fact]
        public void GetBarChartModel_ReturnsModelWithTwoAxes()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetBarChartModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetBarChartModel_UsesAbsoluteValueForBarItems()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = false;
            var items = new List<ReportStructureIncomeExpenseModel>
            {
                new TestModel("Groceries", -200.0),
            };

            var model = testVm.TestGetBarChartModel(items);
            var barSeries = (BarSeries)model.Series[0];

            Assert.Equal(200, barSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetBarChartModel_WithData_AddsLabelsToAxisAndItemsToSeries()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureIncomeExpenseModel>
            {
                new TestModel("Food", 100.0),
                new TestModel("Transport", 50.0),
            };

            var model = testVm.TestGetBarChartModel(items);
            var barSeries = (BarSeries)model.Series[0];
            var categoryAxis = (CategoryAxis)model.Axes[0];

            Assert.Equal(2, barSeries.ActualItems.Count);
            Assert.Equal(2, categoryAxis.ActualLabels.Count);
        }

        [Fact]
        public void GetPlotModel_RaisesBarChartModelPropertyChanged()
        {
            var testVm = CreateTestableVM();
            var raised = false;
            testVm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureIncomeExpenseVM.BarChartModel))
                {
                    raised = true;
                }
            };

            testVm.TestGetPlotModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.True(raised);
        }

        [Fact]
        public void GetPlotModel_ReturnsPieChartWithOneSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.Single(model.Series);
            Assert.IsType<PieSeries>(model.Series[0]);
        }

        [Fact]
        public void GetPlotModel_SetsBarChartModel()
        {
            var testVm = CreateTestableVM();

            testVm.TestGetPlotModel(new List<ReportStructureIncomeExpenseModel>());

            Assert.NotNull(testVm.BarChartModel);
        }

        [Fact]
        public void GetPlotModel_WithData_PieSeriesContainsExpectedSliceCount()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureIncomeExpenseModel>
            {
                new TestModel("Food", 300.0),
                new TestModel("Rent", 500.0),
                new TestModel("Utilities", 120.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)model.Series[0];

            Assert.Equal(3, pieSeries.Slices.Count);
        }

        [Fact]
        public void GetSql_IsIncomeFalse_ContainsLessThanSign()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = false;

            var sql = testVm.TestGetSql();

            Assert.Contains("< 0", sql);
        }

        [Fact]
        public void GetSql_IsIncomeTrue_ContainsGreaterThanSign()
        {
            var testVm = CreateTestableVM();
            testVm.IsIncome = true;

            var sql = testVm.TestGetSql();

            Assert.Contains("> 0", sql);
        }

        [Fact]
        public void GetSql_WithNoFilters_DoesNotContainAndClause()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.DoesNotContain(" and ", sql);
        }

        [Fact]
        public void IsIncome_SetNewValue_RaisesPropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureIncomeExpenseVM.IsIncome))
                {
                    raised = true;
                }
            };

            this.vm.IsIncome = true;

            Assert.True(raised);
        }

        [Fact]
        public void IsIncome_SetSameValue_RaisesPropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureIncomeExpenseVM.IsIncome))
                {
                    raised = true;
                }
            };

            this.vm.IsIncome = false;

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

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureIncomeExpenseModel>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RefreshDataCommand_IsExpense_SqlContainsLessThanSign()
        {
            this.vm.IsIncome = false;
            string capturedSql = null;
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureIncomeExpenseModel>(It.IsAny<string>()))
                .Callback<string>(sql => capturedSql = sql)
                .ReturnsAsync(new List<ReportStructureIncomeExpenseModel>());

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Contains("< 0", capturedSql);
        }

        [Fact]
        public async Task RefreshDataCommand_IsIncome_SqlContainsGreaterThanSign()
        {
            this.vm.IsIncome = true;
            string capturedSql = null;
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureIncomeExpenseModel>(It.IsAny<string>()))
                .Callback<string>(sql => capturedSql = sql)
                .ReturnsAsync(new List<ReportStructureIncomeExpenseModel>());

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Contains("> 0", capturedSql);
        }

        [Fact]
        public async Task RefreshDataCommand_SetsBarChartModel_AfterRefresh()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.NotNull(this.vm.BarChartModel);
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
                .Setup(x => x.ExecuteQuery<ReportStructureIncomeExpenseModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureIncomeExpenseModel>
                {
                    new TestModel("Groceries", 100.0),
                    new TestModel("Transport", 50.0),
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

        private sealed class TestableVM : ReportStructureIncomeExpenseVM
        {
            public TestableVM(IFinancierDatabase db) : base(db)
            {
            }

            public SafePlotModel TestGetBarChartModel(List<ReportStructureIncomeExpenseModel> list) =>
                GetBarChartModel(list);

            public SafePlotModel TestGetPlotModel(List<ReportStructureIncomeExpenseModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ReportStructureIncomeExpenseModel
        {
            public TestModel(string name, double? total)
            {
                Name = name;
                Total = total;
            }
        }
    }
}
