using Microsoft.Data.Sqlite;
using Dapper;

namespace MyBancoApi.ContaCorrente.Infrastructure.Database
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
            // (O schema do ERD fornecido)
            string sql = @"
            CREATE TABLE IF NOT EXISTS contacorrente (
                idcontacorrente TEXT PRIMARY KEY,
                numero TEXT NOT NULL,
                nome TEXT NOT NULL,
                ativo INTEGER NOT NULL,
                senhaHash TEXT NOT NULL,
                salt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS movimento (
                idmovimento TEXT PRIMARY KEY,
                idcontacorrente TEXT NOT NULL,
                datamovimento TEXT NOT NULL,
                tipomovimento CHAR(1) NOT NULL,
                valor REAL NOT NULL,
                FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
            );

            CREATE TABLE IF NOT EXISTS idempotencia (
                chave_idempotencia TEXT PRIMARY KEY,
                requisicao TEXT NOT NULL,
                resultado TEXT
            );
            ";

            _connection.Execute(sql);
        }
    }
}