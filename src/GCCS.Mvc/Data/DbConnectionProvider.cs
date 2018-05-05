using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace GCCS.Mvc.Data
{
    public class DbConnectionProvider
    {
        private readonly string _connectionString;

        public DbConnectionProvider(IConfiguration config)
        {
            var conString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(conString))
            {
                throw new InvalidOperationException(
                    "A non-null non-whitespace connection string must be specified in " +
                    "appsettings.json with the name 'DefaultConnection'.");
            }

            if (!conString.Contains("Pooling=True"))
            {
                throw new InvalidOperationException(
                    "The connection string must specify 'Pooling=True'. " +
                    "This application's performance is highly dependent " +
                    "on database connection pooling.");
            }

            _connectionString = conString;
        }

        public async Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            return connection;
        }
    }
}
