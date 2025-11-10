using Dapper;
using Microsoft.Data.Sqlite;

namespace MyBancoApi.Transferencia.Infrastructure.Database
{
    // Inicializador do Banco de Dados para o microsserviço de Transferência
    public class DbInitializer
    {
        private readonly SqliteConnection _connection;

        public DbInitializer(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void Initialize()
        {
            // Cria as tabelas deste microsserviço (baseado no ERD)
            string sql = @"
            CREATE TABLE IF NOT EXISTS transferencia (
                idtransferencia TEXT PRIMARY KEY,
                idcontacorrente_origem TEXT NOT NULL,
                idcontacorrente_destino TEXT NOT NULL,
                datamovimento TEXT NOT NULL,
                valor REAL NOT NULL
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