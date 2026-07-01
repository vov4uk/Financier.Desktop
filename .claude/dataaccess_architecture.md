# Financier.DataAccess — Architecture Reference

## Purpose

Data access layer: EF Core (SQLite in-memory) DbContext, generic repository/UoW, all entity types, and the main `IFinancierDatabase` façade used by every page VM.

## Entity hierarchy

```
Entity (abstract, empty base)
├── IIdentity (interface: int Id, long UpdatedOn)
├── Tag (abstract) : Entity, IIdentity  ← Payee, Location, Project
└── Concrete entities implementing Entity, IIdentity
```

All monetary values are `long` (fixed-point, smallest currency unit — divide by 100 for display).
All timestamps are `long` (Unix milliseconds).
Amounts/money never stored as float/decimal.

## All entity types and table names

| Entity | Table | Key |
|---|---|---|
| Account | `account` | Id |
| Transaction | `transactions` | Id |
| Category | `category` | Id |
| Currency | `currency` | Id |
| Budget | `budget` | Id |
| Payee | `payee` | Id |
| Location | `locations` | Id |
| Project | `project` | Id |
| AttributeDefinition | `attributes` | Id |
| TransactionAttribute | `transaction_attribute` | (TransactionId, AttributeId) |
| CategoryAttribute | `category_attribute` | keyless |
| CurrencyExchangeRate | `currency_exchange_rate` | (FromCurrencyId, ToCurrencyId, Date) |
| RunningBalance | `running_balance` | (TransactionId, AccountId) |
| CCardClosingDate | `ccard_closing_date` | keyless |
| SmsTemplate | `sms_template` | Id |
| BlotterTransactions | `v_blotter` (view) | Id |
| BlotterTransactionsForAccountWithSplits | `v_blotter_for_account_with_splits` (view) | Id |

`[Table]` / `[Column]` / `[Ignore]` attributes (from `DataAccess.Utils`) drive both EF Core mapping and `Financier.Adapter` backup serialization.

## Key Transaction fields

- `ParentId` — self-ref FK; subtransactions have ParentId > 0
- `FromAccountId` / `ToAccountId` — transfer if ToAccountId > 0
- `CategoryId == -1` → split transaction
- `CategoryId == 0` → no category / transfer
- `Status = "UR"` (default)

## IFinancierDatabase — main façade

```csharp
public interface IFinancierDatabase : IUnitOfWorkFactory, IDisposable
{
    Task ImportEntitiesAsync(IEnumerable<Entity> entities);
    Task RebuildAccountBalanceAsync(int accountId);
    Task AddTransactionsAsync(IEnumerable<Transaction> transactions);
    Task<T> GetOrCreateAsync<T>(int id) where T : class, IIdentity, new();
    Task<List<T>> ExecuteQuery<T>(string query) where T : class, new();
    Task<Transaction> GetOrCreateTransactionAsync(int id);
    Task<IEnumerable<Transaction>> GetSubTransactionsAsync(int id);
    Task InsertOrUpdateAsync<T>(IEnumerable<T> entities) where T : Entity, IIdentity;
    Task SaveAsFile(string dest);
    IUnitOfWork CreateUnitOfWork();
}
```

`GetOrCreateAsync<T>(0)` or id not found → returns new() with defaults (never null).

## Repository / Unit of Work pattern

```
IUnitOfWorkFactory
  └─ CreateUnitOfWork() → IUnitOfWork
       └─ GetRepository<T>() → IBaseRepository<T>
            ├─ AddAsync / Add / AddRangeAsync
            ├─ GetAllAsync / FindByAsync / FindManyAsync
            ├─ FindManyAndProjectAsync<TResult>
            ├─ UpdateAsync
            └─ DeleteAsync
```

`UnitOfWork<TContext>` caches repositories in `ConcurrentDictionary<Type, object>`.

`UnitOfWorkHelper.GetAllAsync<T>(uow, includes...)` — convenience extension.

## In-memory SQLite setup

- `FinancierDatabase()` creates `SqliteConnection("Filename=:memory:")` and holds it alive
- `SeedAsync()` runs embedded SQL scripts: CREATE TABLE → ALTER TABLE → CREATE VIEW
- `EnsureCreated()` is called in UnitOfWork ctor
- Schema is the original Android Financisto SQLite schema

## Factory

```csharp
IFinancierDatabaseFactory.CreateDatabase() → new FinancierDatabase()
```

Always creates a fresh empty DB; populated by `ImportEntitiesAsync` after backup parse.

## Utilities

- `IgnoreAttribute` — marks properties/fields to skip in backup serialization and EF mapping
- `ExpressionExtensions.And<T>()` — combines two Expression predicates with AND
- `UnitOfWorkHelper.GetAllAsync<T>()` — extension on IUnitOfWork

## EF Core config notes

- `AutoDetectChangesEnabled = false` (performance)
- Default tracking: `NoTrackingWithIdentityResolution`
- `ExecuteQuery<T>` uses raw SQL + reflection + `[Column]` attributes to map results to POCOs (with `PropertyCache` ConcurrentDictionary)
