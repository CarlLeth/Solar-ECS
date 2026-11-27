using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;

namespace SolarEcs.Tests.EntityFramework.Infrastructure
{
    /// <summary>
    /// Resolves the SQL Server connection string used by the EF-backed tests.
    ///
    /// Resolution order (first reachable candidate wins, probed once per test run):
    ///   1. SOLAR_TEST_SQL_CONNECTION environment variable
    ///   2. LocalDB default instance — Windows dev machines with SQL Server installed
    ///   3. SQL Server Express — other common Windows dev layout
    ///   4. localhost,1433 — the dockerized SQL Server from docker-compose.yml
    ///
    /// The winner is written to the console so it is obvious which database the
    /// tests ran against.
    /// </summary>
    public static class TestConnection
    {
        public const string DefaultPassword = "SolarTests_2026!";
        public const string DefaultDatabase = "SolarTests";

        private static readonly Lazy<string> ResolvedConnectionString = new Lazy<string>(ResolveConnectionString);

        public static string ConnectionString
        {
            get { return ResolvedConnectionString.Value; }
        }

        private static string ResolveConnectionString()
        {
            var fromEnvironment = Environment.GetEnvironmentVariable("SOLAR_TEST_SQL_CONNECTION");
            if (!string.IsNullOrEmpty(fromEnvironment))
            {
                Console.WriteLine($"Solar test database: SOLAR_TEST_SQL_CONNECTION environment variable");
                return fromEnvironment;
            }

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var candidates = isWindows
                ? new[]
                {
                    $"Server=(localdb)\\MSSQLLocalDB;Database={DefaultDatabase};Trusted_Connection=True",
                    $"Server=.\\SQLEXPRESS;Database={DefaultDatabase};Trusted_Connection=True",
                    $"Server=localhost,1433;Database={DefaultDatabase};User Id=sa;Password={DefaultPassword};TrustServerCertificate=True",
                }
                : new[]
                {
                    $"Server=localhost,1433;Database={DefaultDatabase};User Id=sa;Password={DefaultPassword};TrustServerCertificate=True",
                };

            foreach (var candidate in candidates)
            {
                if (CanConnect(candidate))
                {
                    Console.WriteLine($"Solar test database: {new SqlConnectionStringBuilder(candidate).DataSource}");
                    return candidate;
                }
            }

            throw new InvalidOperationException(
                "Could not reach any SQL Server instance for the Solar tests. Tried:\n" +
                string.Join("\n", candidates.Select(c => "  " + new SqlConnectionStringBuilder(c).DataSource)) +
                "\nSet the SOLAR_TEST_SQL_CONNECTION environment variable to point the tests at a specific instance.");
        }

        private static bool CanConnect(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = 3,
                };

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
