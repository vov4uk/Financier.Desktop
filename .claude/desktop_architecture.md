# Financier.Desktop — Architecture Reference

## Purpose

Main WPF application. Ribbon-based navigation, page VMs, dialog VMs, DTOs, DI wiring. All UI interaction flows through here.

## MainWindowVM

**File:** `src/Financier.Desktop/MainWindowVM.cs`

```csharp
public class MainWindowVM : BindableBase
{
    ConcurrentDictionary<Type, BindableBase> _pages;  // page VM cache by model type
    BindableBase CurrentPage { get; set; }
    bool IsLoading { get; set; }
    string OpenBackupPath { get; set; }
    string DefaultBackupDirectory { get; set; }

    // Ribbon selection flags (one per page):
    bool IsAccountPageSelected { get; }
    // ... IsBlotterPageSelected, IsCurrenciesPageSelected, etc.

    // Commands:
    IAsyncCommand<Type> MenuNavigateCommand
    IAsyncCommand OpenBackupCommand
    IAsyncCommand SaveBackupCommand
    IAsyncCommand SaveBackupAsDbCommand
    IAsyncCommand SettingsCommand
    IAsyncCommand RefreshExchangeRatesCommand
    IAsyncCommand CheckForUpdateCommand
    IAsyncCommand<WizardTypes> ImportCommand
}
```

**Navigation pattern:**
- Ribbon button → `MenuNavigateCommand` with entity Model type as parameter
- `NavigateToType(Type)` → `GetOrCreatePage<TEntity, VMType>()` → lazy `Activator.CreateInstance(VMType, db, dialogWrapper)` → `RefreshCurrentPage()`
- Page type added to `_pages` dict AND as a named bool property for ribbon binding

**Backup open/save:**
- Open: `IEntityReader.ParseBackupFileAsync(path)` → `db.ImportEntitiesAsync(entities)` → `DbManual.SetupAsync(db)` → recreate all pages
- Save: collect entities from all repos → `IBackupWriter.GenerateBackupAsync(entities, path, version, columnsOrder)`

**DI constructor:**
```csharp
MainWindowVM(IDialogWrapper, IFinancierDatabaseFactory, IEntityReader,
             IBackupWriter, IToastNotifierWrapper, IBankHelperFactory, UpdateService)
```

## Page VMs

All in `src/Financier.Desktop/Pages/`.  All inherit `EntityBaseVM<TModel>`.

### EntityBaseVM\<T\> (abstract base)

```csharp
public abstract class EntityBaseVM<T> : BaseViewModel<T> where T : BaseModel, new()
{
    protected IDialogWrapper dialogWrapper;
    public T SelectedValue { get; set; }

    public IAsyncCommand AddCommand
    public IAsyncCommand EditCommand      // CanExecute: SelectedValue != null
    public IAsyncCommand DeleteCommand    // CanExecute: SelectedValue != null

    protected abstract Task OnAdd();
    protected abstract Task OnEdit(T item);
    protected abstract Task OnDelete(T item);
    protected virtual void OnSelectedValueChanged();
}
```

### Page VM summary

| VM | Model | Dialog VM | Extra commands |
|---|---|---|---|
| `AccountsVM` | AccountModel | AccountControlVM | — |
| `BlotterVM` | BlotterModel | TransactionControlVM / TransferControlVM | AddTransaction, AddTransfer, Duplicate, ClearFilters |
| `CategoriesVM` | CategoryTreeModel | CategoryControlVM | MoveTop, MoveUp, MoveDown, MoveBottom, SortByTitle |
| `CurrenciesVM` | CurrencyModel | CurrencyControlVM / AddCurrencyControlVM (template picker) | — |
| `LocationsVM` | LocationModel | LocationControlVM | — |
| `PayeesVM` | PayeeModel | TagControlVM | — |
| `ProjectsVM` | ProjectModel | TagControlVM | — |
| `RulesVM` | RuleModel | RuleControlVM | — (rules stored in rules.json via DbManual) |
| `ExchangeRatesVM` | ExchangeRateModel | none | read-only; OxyPlot graph |

**BlotterVM filters:** From/To dates, Account, Category, Payee, Project, Location  
**CategoriesVM:** Uses nested-set model (Left/Right). Restores expand/select state across refreshes.

## Dialog system

### IDialogWrapper

```csharp
public interface IDialogWrapper
{
    object ShowDialog<T>(DialogBaseVM context, double height, double width, string title = null)
        where T : UserControl, new();
    object ShowWizard(WizardBaseVM context);
    string OpenFileDialog(string fileExtension);
    string SaveFileDialog(string fileExtension, string defaultPath = "");
    bool ShowMessageBox(string text, string caption, bool yesNoButtons = false);
}
```

### DialogBaseVM (abstract base)

```csharp
public abstract class DialogBaseVM : BindableBase
{
    public event EventHandler RequestSave;    // sender = DTO returned by OnRequestSave()
    public event EventHandler RequestCancel;

    public DelegateCommand SaveCommand;
    public DelegateCommand CancelCommand;

    public abstract object OnRequestSave();
    protected virtual bool CanSaveCommandExecute() => true;
}
```

**Flow:**
1. Page VM creates DialogVM with DTO, calls `dialogWrapper.ShowDialog<ControlType>(vm, h, w, title)`
2. DialogHelper creates `Window { Content = new T { DataContext = vm } }` and wires events
3. User saves → `OnRequestSave()` → `RequestSave(dto, EventArgs.Empty)` → window closes, `ShowDialog` returns dto
4. User cancels → window closes, `ShowDialog` returns null
5. Page VM maps dto back to entity, calls `db.InsertOrUpdateAsync([entity])`, then `RefreshData()`

### Dialog VM summary

All in `src/Financier.Desktop/Pages/Dialogs/`. Controls in same folder, `x:Class` `Financier.Desktop.Views.Controls.*`.

| Dialog VM | DTO | Save guard |
|---|---|---|
| `AccountControlVM` | `AccountDto` | Title not empty && CurrencyId > 0 |
| `CategoryControlVM` | `CategoryDto` | — |
| `TransactionControlVM` | `TransactionDto` | FromAccount != null && FromAmount != 0 |
| `SubTransactionControlVM` | `TransactionDto` | `!IsSplitCategory \|\| UnsplitAmount == 0` |
| `TransferControlVM` | `TransferDto` | FromAccount != null && ToAccount != null && From != To |
| `TagControlVM` | `TagDto` | — |
| `LocationControlVM : TagControlVM` | `LocationDto` | — |
| `CurrencyControlVM` | `CurrencyDto` | — |
| `AddCurrencyControlVM` | `CurrencyDto` | — (template picker, then opens CurrencyControlVM) |
| `RuleControlVM` | `RuleDto` | — |

## DTOs (Data Transfer Objects)

All in `src/Financier.Desktop/Data/`. All inherit `BindableBase` (INotifyPropertyChanged).

### Key DTOs

**AccountDto:** Title, Type (enum), CurrencyId, CardIssuer, Issuer, Number, LimitAmount, SortOrder, IsIncludeIntoTotals, Note, ClosingDay, PaymentDay, OpeningAmount

**TransactionDto : BaseTransactionDto:**
- FromAccountId, FromAccount, CategoryId, Category, PayeeId, ProjectId, LocationId
- OriginalCurrencyId, OriginalFromAmount, FromAmount
- `ObservableCollection<BaseTransactionDto> SubTransactions`
- Computed: `IsSplitCategory` (CategoryId==-1), `UnsplitAmount`, `SplitAmount`, `RateString`

**TransferDto : BaseTransactionDto:**
- FromAccountId, FromAccount, ToAccountId, ToAccount, FromAmount, ToAmount
- Computed: `IsToAmountVisible`, `RateString`
- `RealFromAmount` always negative

**BaseTransactionDto:** DateTime (Date + Time combined), Id, Note, IsSubTransaction

**TagDto:** Title, IsActive  
**LocationDto : TagDto** + Address  
**CurrencyDto:** Title, Name, Symbol, Decimals, DecimalSeparator, GroupSeparator, SymbolFormat, NumberFormat, IsDefault, IsActive  
**RuleDto:** Description, Condition (RuleConditionType enum), CategoryId, LocationId, PayeeId, ProjectId, MCCCategory, Created

## Ribbon navigation — adding a new page

1. Create `EntityNameVM : EntityBaseVM<EntityNameModel>` in `Pages/`
2. Register model type → VM in `MainWindowVM._pages` bootstrapping (via `GetOrCreatePage<EntityModel, EntityNameVM>()`)
3. Add `bool IsEntityNamePageSelected => CurrentPage is EntityNameVM` property
4. Add ribbon button binding `Command="{Binding MenuNavigateCommand}" CommandParameter="{x:Type data:EntityNameModel}"`
5. Add ribbon group `Visibility="{Binding IsEntityNamePageSelected, Converter=...}"`

## Key service interfaces

- `IDialogWrapper` → `DialogHelper` — all modal dialogs and file pickers
- `IFinancierDatabaseFactory` → `FinancierDatabaseFactory` — creates IFinancierDatabase
- `IEntityReader` — backup file parsing (from Financier.Adapter)
- `IBackupWriter` — backup file generation (from Financier.Adapter)
- `IToastNotifierWrapper` — toast notifications
- `IBankHelperFactory` — import wizard creation (ABank, Monobank, Pumb, Revolut)
