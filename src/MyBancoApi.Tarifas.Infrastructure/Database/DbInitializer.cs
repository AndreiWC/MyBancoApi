using Dapper;
using Microsoft.Data.Sqlite;

namespace MyBancoApi.Tarifas.Infrastructure.Database
{
    public class DbInitializer
    {
        private readonly SqliteConnection _connection;

        public DbInitializer(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Initialize()
        {
            // (O schema do ERD fornecido para tarifas)
            string sql = @"
            CREATE TABLE IF NOT EXISTS tarifas (
                idtarifa TEXT PRIMARY KEY,
                idcontacorrente TEXT NOT NULL,
                datamovimento TEXT NOT NULL,
                valor REAL NOT NULL
            );";

            _connection.Execute(sql);
        }
    }
}