namespace Financier.Reports.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Moq;
    using OxyPlot.Series;
    using Xunit;

    public class ReportStructureActivesVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ReportStructureActivesVM vm;

        public ReportStructureActivesVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureActivesModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureActivesModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ReportStructureActivesVM(this.dbMock.Object);
            this.vm.StartYearMonths = new YearMonths();
            this.vm.EndYearMonths = new YearMonths();
        }

        // ── Constructor ──────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_SetsDateFilter_ToApproximatelyNow()
        {
            Assert.True(this.vm.DateFilter!.Value >= DateTime.Now.AddSeconds(-5));
        }

        [Fact]
        public void Constructor_SetsDateFilter_ToNonNull()
        {
            Assert.NotNull(this.vm.DateFilter);
        }
        // ── GetSql ───────────────────────────────────────────────────────────────

        [Fact]
        public void GetPlotModel_EmptyList_ReturnsEmptyPieSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportStructureActivesModel>());
            var pieSeries = (PieSeries)model.Series[0];

            Assert.Empty(pieSeries.Slices);
        }

        [Fact]
        public void GetPlotModel_ExcludesAccountsNotIncludedInTotals()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureActivesModel>
            {
                new TestModel("Included",    includeInTotals: 1, balance: 1000.0),
                new TestModel("Excluded",    includeInTotals: 0, balance: 500.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)model.Series[0];

            Assert.Single(pieSeries.Slices);
        }

        [Fact]
        public void GetPlotModel_ExcludesAccountsWithNonPositiveBalance()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureActivesModel>
            {
                new TestModel("Positive",  includeInTotals: 1, balance: 100.0),
                new TestModel("Zero",      includeInTotals: 1, balance: 0.0),
                new TestModel("Negative",  includeInTotals: 1, balance: -50.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)model.Series[0];

            Assert.Single(pieSeries.Slices);
        }

        [Fact]
        public void GetPlotModel_NoSmallSlices_DoesNotAddOthers()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureActivesModel>
            {
                new TestModel("A", includeInTotals: 1, balance: 500.0),
                new TestModel("B", includeInTotals: 1, balance: 500.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)model.Series[0];

            Assert.Equal(2, pieSeries.Slices.Count);
        }

        [Fact]
        public void GetPlotModel_ReturnsModelWithOnePieSeries()
        {
            var testVm = CreateTestableVM();

            var model = testVm.TestGetPlotModel(new List<ReportStructureActivesModel>());

            Assert.Single(model.Series);
            Assert.IsType<PieSeries>(model.Series[0]);
        }

        // ── GetPlotModel ─────────────────────────────────────────────────────────
        [Fact]
        public void GetPlotModel_SmallSlices_GroupedIntoOthers()
        {
            var testVm = CreateTestableVM();
            var items = new List<ReportStructureActivesModel>
            {
                new TestModel("Big",   includeInTotals: 1, balance: 9990.0),
                new TestModel("Small", includeInTotals: 1, balance: 1.0),
            };

            var model = testVm.TestGetPlotModel(items);
            var pieSeries = (PieSeries)model.Series[0];

            // "Big" slice + "others" slice (Small is < 1%)
            Assert.Equal(2, pieSeries.Slices.Count);
            Assert.Equal(9990.0, pieSeries.Slices[0].Value);
        }

        [Fact]
        public void GetSql_WithDateFilter_ContainsUnixTimestamp()
        {
            var testVm = CreateTestableVM();
            var expectedTimestamp = new DateTimeOffset(testVm.DateFilter!.Value).ToUnixTimeMilliseconds().ToString();

            var sql = testVm.TestGetSql();

            Assert.Contains(expectedTimestamp, sql);
        }

        [Fact]
        public void GetSql_WithDateFilter_ReturnsNonEmptySql()
        {
            var testVm = CreateTestableVM();

            var sql = testVm.TestGetSql();

            Assert.NotEmpty(sql);
        }

        // ── RefreshDataCommand ───────────────────────────────────────────────────

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

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureActivesModel>(It.IsAny<string>()), Times.Once);
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
                .Setup(x => x.ExecuteQuery<ReportStructureActivesModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureActivesModel>
                {
                    new TestModel("Savings", includeInTotals: 1, balance: 500.0),
                    new TestModel("Wallet", includeInTotals: 1, balance: 200.0),
                });

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(2, this.vm.Entities.Count);
        }

        // ── Helper methods ───────────────────────────────────────────────────────

        private TestableVM CreateTestableVM()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.StartYearMonths = new YearMonths();
            testVm.EndYearMonths = new YearMonths();
            return testVm;
        }

        // ── Helper types ─────────────────────────────────────────────────────────

        private sealed class TestableVM : ReportStructureActivesVM
        {
            public TestableVM(IFinancierDatabase db) : base(db)
            {
            }

            public SafePlotModel TestGetPlotModel(List<ReportStructureActivesModel> list) =>
                GetPlotModel(list);

            public string TestGetSql() => GetSql();
        }

        private sealed class TestModel : ReportStructureActivesModel
        {
            public TestModel(string title, long includeInTotals, double? balance)
            {
                Title = title;
                AccountIsIncludeInTotals = includeInTotals;
                DefaultCurrencyBalance = balance;
            }
        }
    }
}
