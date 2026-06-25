# Financier.Reports & Testing — Architecture Reference

## Financier.Reports

**Location:** `src/Financier.Reports/`

### Architecture

```
ReportsControlVM
  └─ dynamically instantiates report VMs via reflection
       └─ BaseReportVM<T> (abstract)
            ├─ GetSql() → string (abstract, per report)
            ├─ GetPlotModel() → PlotModel (abstract, per report)
            ├─ GetStandartTrnFilter() → SQL WHERE clause (shared)
            └─ Filter state: Project, Category, TopCategory, Account, Payee, Currency, Date range
```

### Report VM classes

| Class | Chart type | Focus |
|---|---|---|
| `ReportByPeriodMonthCrcVM` | Line + Bar | Income/Expense/Saldo by month |
| `ReportStructureActivesVM` | Pie/Bar | Asset structure |
| `ReportStructureIncomeExpenseVM` | Pie/Bar | Income vs Expense breakdown |
| `ByCategoryReportVM` | Horizontal bar + Pie | Spending by category |
| `ReportStructureSaldoVM` | Pie | Account balance structure |
| `ReportDynamicDebitCretitPayeeVM` | Line | Dynamic debit/credit trends over time |
| `ReportDynamicRestVM` | Line | Account balance dynamics |

`ReportsControlVM` builds a `TreeNode`-based navigation tree; selecting a node instantiates the matching VM via `Activator.CreateInstance(type, IFinancierDatabase)`.

`SafePlotModel` — wrapper around OxyPlot `PlotModel` that prevents double-attachment to views via reflection cleanup.

---

## Test infrastructure

### Financier.Tests.Common

**Location:** `src/Tests/Financier.Tests.Common/`

**Key classes:**

| Class | Purpose |
|---|---|
| `DataFixture` | AutoFixture with AutoMoq; disables recursive behaviour |
| `AutoMoqDataAttribute` | `[Theory]` data source attribute (Xunit3) for auto-mocked + generated data |
| `PredefinedData` | Static test entities: `PredefinedData.Transaction`, `PredefinedData.TransactionsColumnsOrder` |
| `TestComparer` | `IEqualityComparer<Transaction>` — deep equality on all 24 Transaction properties |
| `ColumnJsonPropertyResolver<T>` | Newtonsoft.Json contract resolver: maps C# property names → `[Column]` DB names |
| `JsonDeserializer` | Deserializes JSON to entity lists using `ColumnJsonPropertyResolver` |

**Usage pattern:**
```csharp
[Theory]
[AutoMoqData]
public async Task MyTest(string input, Mock<IMyService> mockService, MyEntity entity)
{ ... }
```

---

### Financier.DataAccess.Tests

**Location:** `src/Tests/Financier.DataAccess.Tests/`

Tests `FinancierDatabase` with a real in-memory SQLite database (no mocking of the DB layer).

| Test area | Key scenarios |
|---|---|
| Repository creation | All 14 repositories available via UoW |
| AddTransactionsAsync | Batch insert, duplicate detection |
| GetOrCreateAsync | Returns existing / null-safe new() / correct defaults |
| InsertOrUpdateAsync | UPSERT: new entities added, existing updated |
| RebuildRunningBalanceForAccount | Balance calculation across 3 transactions, `Account.TotalAmount` updated |
| ImportEntitiesAsync | Multi-type entity import including RunningBalance calc |
| GetSubTransactionsAsync | Parent-child navigation |

**Pattern:**
```csharp
var db = new FinancierDatabase();   // fresh in-memory SQLite
using var uow = db.CreateUnitOfWork();
var repo = uow.GetRepository<Account>();
await repo.AddAsync(entity);
await uow.SaveChangesAsync();
```

---

### Financier.Desktop.Tests

**Location:** `src/Tests/Financier.Desktop.Tests/`

Integration tests with `Mock<IDialogWrapper>(MockBehavior.Strict)` and a real database.

| Test class | Coverage |
|---|---|
| `MainWindowVMTest` (19 tests) | Open backup, save backup, import wizards (ABank/Monobank/Pumb), transaction/transfer CRUD, running balances |
| `BlotterVMIntegrationTests` (10+ tests) | Filter combinations, duplicate detection, split transactions, transfers, balance updates |
| `TransactionDialogVMTest` | Sub-transaction CRUD, recipes dialog, amount sign handling |
| `MonoWizardVMTest` + `Page1-3VMTest` | Monobank CSV import wizard flow |
| `RecipesVMTests` + `Page1-2VMTest` | Recipe-based transaction splitting |
| `RevolutHelperTest` | Revolut-specific CSV/statement parsing |

**Dialog mock pattern:**
```csharp
var dialogMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
TransactionDto? capturedDto = null;

dialogMock.Setup(x => x.ShowDialog<TransactionControl>(
    It.IsAny<TransactionControlVM>(), height, width, title))
    .Callback<TransactionControlVM, double, double, string>(
        (vm, h, w, t) => capturedDto = vm.Entity)
    .Returns(() => capturedDto);  // simulate Save
```

**JSON assertion pattern:**
```csharp
var result = await db.ExecuteQuery<BlotterModel>(sql);
Assert.Equal(expectedJson, JsonConvert.SerializeObject(result, settings));
```

Test assets in `src/Tests/Financier.Adapter.Tests/Assets/`:
- `min.backup` — sample Financisto backup (gzip)
- `min` — same content uncompressed (round-trip reference)
- `mono.ukr.csv` — Monobank export
- `abank.pdf`, `pumb.pdf` — bank statement PDFs

---

### Financier.Converter.Test

**Location:** `src/Tests/Financier.Converter.Test/`

Pure unit tests for WPF value converters. No DI, no DB.

| Test class | Converter |
|---|---|
| `AmountConverterTest` | Convert (÷100) / ConvertBack (×100), sign param |
| `UnixTimeConverterTest` | DateTime ↔ Unix ms, timezone |
| `InverseBooleanConverterTest` | bool negation |
| `InvertedBooleanToVisibilityConverterTest` | bool → Visibility |
| `MccConverterTest` | MCC int → category name |
| `StringEmptyToVisibilityConverterTest` | null/empty → Collapsed |

Pattern: `[Theory]` + `[InlineAutoData]` + `CultureInfo.InvariantCulture`.

---

## Running tests

```powershell
dotnet test src/Tests/Financier.Adapter.Tests/
dotnet test src/Tests/Financier.DataAccess.Tests/
dotnet test src/Tests/Financier.Desktop.Tests/
dotnet test src/Tests/Financier.Converter.Test/
```

Test project framework: **xunit.v3** + **AutoFixture.AutoMoq** + **Moq**.
