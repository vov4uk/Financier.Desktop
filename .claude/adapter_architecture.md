# Financier.Adapter — Architecture Reference

## Purpose

`Financier.Adapter` reads and writes Financisto Android `.backup` files — gzip-compressed line-based text. It is the entry point for all data: user opens a backup → `EntityReader` parses it → in-memory EF Core DB is populated.

## Backup file format

```
PACKAGE:ru.orangesoftware.financisto
VERSION_CODE:100
VERSION_NAME:1.7.4
DATABASE_VERSION:211
#START
$ENTITY:transactions
_id:1
from_account_id:2
...
$$
$ENTITY:account
...
$$
#END
```

- Header lines are `KEY:VALUE` pairs before `#START`
- Entity blocks: `$ENTITY:<table>` → zero or more `column:value` lines → `$$`
- `Line` struct splits on first `:` only (values may contain `:`)

## Key classes

| Class | Role |
|---|---|
| `BackupReader` | Low-level: opens gzip, reads header into `BackupVersion`, yields body lines as `IAsyncEnumerable<string>` |
| `EntityReader` | High-level: calls `BackupReader`, maps lines to `Entity` instances, tracks column order |
| `BackupWriter` | High-level: serializes `IEnumerable<Entity>` back to gzip backup, preserving column order |
| `EntityExtensions.WriteBackupLines()` | Extension on `Entity` — does the actual per-entity serialization |
| `EntityInfo` | Per-type metadata: factory `Func<Entity>` + `Dictionary<string, EntityPropertyInfo>` |
| `EntityPropertyInfo` | Per-property compiled setter (Expression tree) + `IPropertyConverter` |
| `DefaultConverter` | Converts string↔CLR types; bool stored as 0/1, double/float uses invariant `.` decimal |
| `Line` (struct) | Splits `"key:value"` on first `:` |
| `BackupVersion` | Header metadata: Package, VersionCode, Version, DatabaseVersion |

## Entity discovery (EntityReader.BuildEntityTypes)

Auto-discovers all types in `Financier.DataAccess` assembly that:
1. Inherit `Entity` (abstract base, no fields)
2. Have `[Table("tablename")]` attribute → keyed by table name
3. Per property: skip if `[Ignore]`, include if `[Column("colname")]` → keyed by column name

Uses compiled `Expression.Lambda<Func<Entity>>` factories (no reflection overhead at runtime).

## Column order preservation

Critical for round-trip fidelity — the writer must emit columns in the same order they appeared in the original backup.

`EntityReader.ParseBackupFileAsync` builds:
```csharp
Dictionary<string, List<string>> EntityColumnsOrder  // tableName → ordered column list
```

Tracks insertion order using `HashSet<string> columnsSeen` + `prevField` pointer for `List.Insert(IndexOf(prevField)+1, ...)`.

This dict is passed through to `BackupWriter.GenerateBackupAsync` → `BuildColumnData` → `Dictionary<string, (Index, Count)>` → slot-indexed `string[]` per entity.

## Export order (BackupWriter)

Fixed type order matters for Financisto compatibility:
```
Account → AttributeDefinition → CategoryAttribute → TransactionAttribute →
Budget → Category → Currency → Location → Project →
Transaction → Payee → CCardClosingDate → SmsTemplate → CurrencyExchangeRate
```

## Adding a new entity type

1. Create class in `Financier.DataAccess/Data/` inheriting `Entity`
2. Add `[Table("tablename")]` on the class
3. Add `[Column("colname")]` on each property to export; `[Ignore]` to skip
4. Add the type to `BackupWriter.ExportOrder` array at the correct position

No other changes needed — `EntityReader` discovers it automatically.

## Adding a custom converter

Implement `IPropertyConverter`:
```csharp
public interface IPropertyConverter {
    Type PropertyType { get; set; }
    object Convert(object value);      // string → CLR type (reading)
    string ConvertBack(object value);  // CLR type → string (writing)
}
```

Assign in `EntityReader.BuildEntityTypes` instead of `new DefaultConverter { ... }`.

Currently `DefaultConverter` handles: `bool` (0/1), `double`, `float` (invariant `.`), `IIdentity` (serialized as `.Id`), all other types via `Convert.ChangeType`.

## Test pattern (Financier.Adapter.Tests)

- `BackupReaderTests` — raw line count + header values from `Assets/min.backup`
- `EntityReaderTests` — entity counts by type + column order + specific entity deep-equal via `TestComparer`
- `BackupWriterTests` — round-trip: parse `min.backup` → write `actual.backup` → decompress → compare text against `Assets/min` (uncompressed reference)
- Test assets: `Assets/min.backup` (gzip), `Assets/min` (plain text reference)

## IEntityReader / IBackupWriter interfaces

```csharp
// IEntityReader
Task<(IEnumerable<Entity>, BackupVersion, Dictionary<string, List<string>>)> ParseBackupFileAsync(string fileName);

// IBackupWriter
Task GenerateBackupAsync(IEnumerable<Entity> entities, string fileName, BackupVersion backupVersion, Dictionary<string, List<string>> entityColumnsOrder);
```

Both registered in DI; `BackupWriter.GenerateFileName()` produces `"yyyyMMdd_HHmmss_fff.backup"` timestamp names.
