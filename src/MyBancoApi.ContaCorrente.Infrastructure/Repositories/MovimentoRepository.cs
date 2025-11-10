using Dapper;
using Microsoft.Data.Sqlite;
using MyBancoApi.ContaCorrente.Domain.Entities;
using MyBancoApi.ContaCorrente.Domain.Interfaces;

namespace MyBancoApi.ContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly SqliteConnection _dbConnection;

        public MovimentoRepository(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<decimal> GetSaldoAsync(string idContaCorrente)
        {
            const string sql = @"
                SELECT 
                    COALESCE(SUM(
                        CASE 
                            WHEN tipomovimento = 'C' THEN valor 
                            WHEN tipomovimento = 'D' THEN -valor 
                            ELSE 0 
                        END
                    ), 0) AS Saldo
                FROM movimento
                WHERE idcontacorrente = @IdContaCorrente;
            ";

            var saldo = await _dbConnection.ExecuteScalarAsync<decimal>(sql, new { IdContaCorrente = idContaCorrente });
            return Math.Round(saldo, 2);
        }
        public async Task SalvarAsync(Movimento movimento)
        {
            string sql = @"
                INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);
            ";
            await _dbConnection.ExecuteAsync(sql, movimento);
        }
    }
}