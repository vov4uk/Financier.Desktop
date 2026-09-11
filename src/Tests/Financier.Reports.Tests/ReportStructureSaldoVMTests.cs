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

    public class ReportStructureSaldoVMTests
    {
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly ReportStructureSaldoVM vm;

        public ReportStructureSaldoVMTests()
        {
            this.dbMock = new Mock<IFinancierDatabase>();
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>());

            DbManual.SetupTests(new List<CurrencyModel> { new CurrencyModel() });
            DbManual.SetupTests(new List<AccountFilterModel> { new AccountFilterModel() });
            DbManual.SetupTests(new List<CategoryModel> { new CategoryModel() });
            DbManual.SetupTests(new List<ProjectModel> { new ProjectModel() });
            DbManual.SetupTests(new List<PayeeModel> { new PayeeModel() });

            this.vm = new ReportStructureSaldoVM(this.dbMock.Object);
        }

        [Fact]
        public void Constructor_SetsDefaultIsUsdCurrencySelected_ToTrue()
        {
            Assert.True(this.vm.IsUsdCurrencySelected);
        }

        [Fact]
        public void Constructor_SetsDefaultRange_ToLast6Months()
        {
            Assert.Equal(ReportStructureSaldoRange.Last6Months, this.vm.Range);
        }

        [Fact]
        public void GetBarChartModel_IsUsdCurrencySelected_UsesUsdBalance_ForAssets()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = true;
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { AssetsUSDBalance = 100, AssetsDefaultCurrencyBalance = 200, Date = DateOnly.FromDateTime(DateTime.Today) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var assetsSeries = (BarSeries)model.Series[0];

            Assert.Equal(100, assetsSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetBarChartModel_IsUsdCurrencySelected_UsesUsdBalance_ForLiabilities()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = true;
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { LiabilitiesUSDBalance = -100, LiabilitiesDefaultCurrencyBalance = -200, Date = DateOnly.FromDateTime(DateTime.Today) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var liabilitiesSeries = (BarSeries)model.Series[1];

            Assert.Equal(-100, liabilitiesSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetBarChartModel_IsUsdCurrencySelected_UsesUsdBalance_ForNetWorth()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = true;
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { NetWorthUSDBalance = 300, NetWorthDefaultCurrencyBalance = 800, Date = DateOnly.FromDateTime(DateTime.Today) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var netWorthSeries = (LineSeries)model.Series[2];

            Assert.Equal(300, netWorthSeries.Points[0].Y);
        }

        [Fact]
        public void GetBarChartModel_NotUsdCurrencySelected_UsesDefaultCurrencyBalance_ForAssets()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = false;
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { AssetsUSDBalance = 100, AssetsDefaultCurrencyBalance = 200, DefaultCurrencySymbol = "UAH", Date = DateOnly.FromDateTime(DateTime.Today) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var assetsSeries = (BarSeries)model.Series[0];

            Assert.Equal(200, assetsSeries.ActualItems[0].Value);
        }

        [Fact]
        public void GetBarChartModel_NotUsdCurrencySelected_UsesDefaultCurrencyBalance_ForNetWorth()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = false;
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { NetWorthUSDBalance = 300, NetWorthDefaultCurrencyBalance = 800, DefaultCurrencySymbol = "UAH", Date = DateOnly.FromDateTime(DateTime.Today) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var netWorthSeries = (LineSeries)model.Series[2];

            Assert.Equal(800, netWorthSeries.Points[0].Y);
        }

        [Fact]
        public void GetBarChartModel_ReturnsModelWithThreeSeries()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var model = testVm.TestGetBarChartModel(new List<ReportStructureSaldoModel>());

            Assert.Equal(3, model.Series.Count);
        }

        [Fact]
        public void GetBarChartModel_ReturnsModelWithTwoAxes()
        {
            var testVm = new TestableVM(this.dbMock.Object);

            var model = testVm.TestGetBarChartModel(new List<ReportStructureSaldoModel>());

            Assert.Equal(2, model.Axes.Count);
        }

        [Fact]
        public void GetBarChartModel_SortsItemsByDateAscending()
        {
            var testVm = new TestableVM(this.dbMock.Object);
            testVm.IsUsdCurrencySelected = true;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var items = new List<ReportStructureSaldoModel>
            {
                new ReportStructureSaldoModel { NetWorthUSDBalance = 200, Date = today },
                new ReportStructureSaldoModel { NetWorthUSDBalance = 100, Date = today.AddMonths(-1) },
            };

            var model = testVm.TestGetBarChartModel(items);
            var netWorthSeries = (LineSeries)model.Series[2];

            Assert.Equal(100, netWorthSeries.Points[0].Y);
            Assert.Equal(200, netWorthSeries.Points[1].Y);
        }

        [Fact]
        public void IsUsdCurrencySelected_SetNewValue_RaisesPropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureSaldoVM.IsUsdCurrencySelected))
                {
                    raised = true;
                }
            };

            this.vm.IsUsdCurrencySelected = false;

            Assert.True(raised);
        }

        [Fact]
        public void IsUsdCurrencySelected_SetSameValue_DoesNotRaisePropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureSaldoVM.IsUsdCurrencySelected))
                {
                    raised = true;
                }
            };

            this.vm.IsUsdCurrencySelected = true;

            Assert.False(raised);
        }

        [Fact]
        public void Range_SetNewValue_RaisesPropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureSaldoVM.Range))
                {
                    raised = true;
                }
            };

            this.vm.Range = ReportStructureSaldoRange.Last12Months;

            Assert.True(raised);
        }

        [Fact]
        public void Range_SetSameValue_DoesNotRaisePropertyChanged()
        {
            var raised = false;
            this.vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ReportStructureSaldoVM.Range))
                {
                    raised = true;
                }
            };

            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            Assert.False(raised);
        }

        [Fact]
        public async Task RefreshDataCommand_AccountExcludedFromTotals_NotIncludedInBalance()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "BANK", defaultCurrencyBalance: 100.0, usdBalance: 50.0),
                    new TestRawModel(includeInTotals: false, accountType: "BANK", defaultCurrencyBalance: 9999.0, usdBalance: 9999.0),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(100, this.vm.Entities[0].AssetsDefaultCurrencyBalance);
            Assert.Equal(50, this.vm.Entities[0].AssetsUSDBalance);
        }

        [Fact]
        public async Task RefreshDataCommand_CalculatesNetWorth_AsAssetsPlusLiabilities()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "CASH", defaultCurrencyBalance: 1000.0, usdBalance: 400.0),
                    new TestRawModel(includeInTotals: true, accountType: "LIABILITY", defaultCurrencyBalance: -200.0, usdBalance: -80.0),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(800, this.vm.Entities[0].NetWorthDefaultCurrencyBalance);
            Assert.Equal(320, this.vm.Entities[0].NetWorthUSDBalance);
        }

        [Fact]
        public async Task RefreshDataCommand_CurrentYear_ExecutesQueryForEachMonthInCurrentYear()
        {
            this.vm.Range = ReportStructureSaldoRange.CurrentYear;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()), Times.Exactly(DateTime.Today.Month));
        }

        [Fact]
        public async Task RefreshDataCommand_EmptyQueryResult_EntitiesIsEmpty()
        {
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Empty(this.vm.Entities);
        }

        [Fact]
        public async Task RefreshDataCommand_Last12Months_ExecutesQueryTwelveTimes()
        {
            this.vm.Range = ReportStructureSaldoRange.Last12Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()), Times.Exactly(12));
        }

        [Fact]
        public async Task RefreshDataCommand_Last24Months_ExecutesQueryTwentyFourTimes()
        {
            this.vm.Range = ReportStructureSaldoRange.Last24Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()), Times.Exactly(24));
        }

        [Fact]
        public async Task RefreshDataCommand_Last2Years_ExecutesQueryFromCurrentMonthToJanuaryOfPreviousYear()
        {
            this.vm.Range = ReportStructureSaldoRange.Last2Years;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            var expectedCount = DateTime.Today.Month + 12;
            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()), Times.Exactly(expectedCount));
        }

        [Fact]
        public async Task RefreshDataCommand_Last6Months_ExecutesQuerySixTimes()
        {
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()), Times.Exactly(6));
        }

        [Fact]
        public async Task RefreshDataCommand_SetsDefaultCurrencySymbol_FromFirstRawRow()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "CASH", defaultCurrencyBalance: 1.0, usdBalance: 1.0, defaultCurrencySymbol: "UAH"),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal("UAH", this.vm.Entities[0].DefaultCurrencySymbol);
        }

        [Fact]
        public async Task RefreshDataCommand_SetsPlotModel_AfterRefresh()
        {
            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.NotNull(this.vm.PlotModel);
        }

        [Fact]
        public async Task RefreshDataCommand_WithAssetRows_SumsAssetsBalances()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "CASH", defaultCurrencyBalance: 100.0, usdBalance: 50.0, defaultCurrencySymbol: "UAH"),
                    new TestRawModel(includeInTotals: true, accountType: "BANK", defaultCurrencyBalance: 200.0, usdBalance: 80.0, defaultCurrencySymbol: "UAH"),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(300, this.vm.Entities[0].AssetsDefaultCurrencyBalance);
            Assert.Equal(130, this.vm.Entities[0].AssetsUSDBalance);
        }

        [Fact]
        public async Task RefreshDataCommand_WithData_EntityCountMatchesDateRange()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "CASH", defaultCurrencyBalance: 100.0, usdBalance: 50.0),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(6, this.vm.Entities.Count);
        }

        [Fact]
        public async Task RefreshDataCommand_WithLiabilityRows_SumsLiabilitiesBalances()
        {
            this.dbMock
                .Setup(x => x.ExecuteQuery<ReportStructureSaldoRawModel>(It.IsAny<string>()))
                .ReturnsAsync(new List<ReportStructureSaldoRawModel>
                {
                    new TestRawModel(includeInTotals: true, accountType: "LIABILITY", defaultCurrencyBalance: -500.0, usdBalance: -200.0),
                    new TestRawModel(includeInTotals: true, accountType: "LIABILITY", defaultCurrencyBalance: -300.0, usdBalance: -100.0),
                });
            this.vm.Range = ReportStructureSaldoRange.Last6Months;

            await this.vm.RefreshDataCommand.ExecuteAsync();

            Assert.Equal(-800, this.vm.Entities[0].LiabilitiesDefaultCurrencyBalance);
            Assert.Equal(-300, this.vm.Entities[0].LiabilitiesUSDBalance);
        }

        private sealed class TestableVM(IFinancierDatabase db) : ReportStructureSaldoVM(db)
        {
            public SafePlotModel TestGetBarChartModel(List<ReportStructureSaldoModel> list) =>
                GetBarChartModel(list);
        }

        private sealed class TestRawModel : ReportStructureSaldoRawModel
        {
            public TestRawModel(
                bool includeInTotals,
                string accountType,
                double? defaultCurrencyBalance = null,
                double? usdBalance = null,
                string defaultCurrencySymbol = null)
            {
                AccountIsIncludeInTotals = includeInTotals;
                AccountType = accountType;
                DefaultCurrencyBalance = defaultCurrencyBalance;
                USDBalance = usdBalance;
                DefaultCurrencySymbol = defaultCurrencySymbol;
            }
        }
    }
}
