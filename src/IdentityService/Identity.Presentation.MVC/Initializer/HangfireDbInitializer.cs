using Microsoft.Data.SqlClient;
using System.Threading;

namespace Identity.Presentation.Initializer
{
    public interface IDatabaseInitializer
    {
        Task EnsureDatabaseCreatedAsync();
    }

    public class HangfireDbInitializer
    {
        private readonly string _connectionString;

        public HangfireDbInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EnsureDatabaseCreatedAsync()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = "HangfireDB";

            builder.InitialCatalog = "master";
            var masterConnection = builder.ToString();

            using var connection = new SqlConnection(masterConnection);
            await connection.OpenAsync();

            var createDbCommand = $@"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                CREATE DATABASE [{databaseName}];
            END";

            using var command = new SqlCommand(createDbCommand, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
