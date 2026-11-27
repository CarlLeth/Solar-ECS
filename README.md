# Solar-ECS

An Entity Component System for .NET, with persistence providers (in-memory and Entity Framework 6 / SQL Server).

## EF SQL Server stress-test suite

`src/SolarEcs.Tests.EntityFramework` exercises the EF6 SQL Server provider end to end (model building, bulk copy, transactions, and LINQ-to-SQL translation of query plans) in preparation for a future migration to EF Core or another ORM.

### Running locally

```bash
docker compose up -d          # starts SQL Server 2022 on localhost:1433 (dev-only sa password)
dotnet test src/SolarEcs.Tests.EntityFramework
```

The connection is resolved automatically in this order (probed once per test run, winner printed to the console):

1. `SOLAR_TEST_SQL_CONNECTION` environment variable, if set
2. LocalDB default instance `(localdb)\MSSQLLocalDB` (Windows only)
3. SQL Server Express `.\SQLEXPRESS` (Windows only)
4. `localhost,1433` — the dockerized instance from `docker-compose.yml`

So on a Windows machine with SQL Server (LocalDB) installed the tests run against it with no configuration; on Linux/CI they use the container.

To point the tests at a different SQL Server instance, set the connection string:

```powershell
$env:SOLAR_TEST_SQL_CONNECTION = "Server=(localdb)\MSSQLLocalDB;Database=SolarTests;Trusted_Connection=True"
dotnet test src/SolarEcs.Tests.EntityFramework
```

```bash
export SOLAR_TEST_SQL_CONNECTION="Server=localhost,1433;Database=SolarTests;User Id=sa;Password=SolarTests_2026!;TrustServerCertificate=True"
dotnet test src/SolarEcs.Tests.EntityFramework
```

(LocalDB is Windows-only, so on Linux the bash example targets the dockerized SQL Server instead.)

CI runs the same suite against a SQL Server service container (see `.github/workflows/tests.yml`).

### Known bugs surfaced by the suite

Two defects in `SqlBulkTransaction` are pinned by tests that are marked `[Ignore]` until fixed:

1. `GetDeleteSql` interpolates the entities collection object into the SQL string instead of the formatted id list (`Sql/SqlBulkTransaction.cs`). Currently dead code — the SQL Server path uses the table-valued-parameter delete instead — but wrong.
2. `BulkDeleteComponentsBatches` (the delete path when `DatabaseMode` is `Generic`) builds batches of 1000 ids and command objects but never executes any command, so unassignments are silently dropped in Generic mode.

Remove the `[Ignore]` attributes on `GetDeleteSqlFormatsIdListIntoSql` and `GenericDatabaseModeDeletesComponents` (in `Persistence/BulkPathTests.cs`) when fixing them.

### Notes for a future ORM migration

- EF6 caches the compiled model per context type within an AppDomain; every context of the same type must therefore register the same component set. Test classes that need different mapping/settings use dedicated context subclasses or custom `ISolarSqlSettings` on a separate context instance.
- Writes go through `SqlBulkCopy` + a TVP delete (`EntityListType`) by default; the non-bulk path (`UseBulkUploading = false`) goes through EF change tracking (`DbSetTransaction`). Both are covered.
- `SharedSqlContext` manages a second `SqlConnection`/`SqlTransaction` alongside EF's own connection; its commit/rollback lifecycle is covered by `TransactionTests`.
