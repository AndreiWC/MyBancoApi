using Dapper;
using Microsoft.Data.Sqlite;
using MyBancoApi.Transferencia.Domain.Entities;
using MyBancoApi.Transferencia.Domain.Interfaces;
using System.Threading.Tasks;

namespace MyBancoApi.Transferencia.Infrastructure.Repositories
{
    // Implementação do Repositório de Idempotência para este microsserviço
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly SqliteConnection _dbConnection;

        public IdempotenciaRepository(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Idempotencia> GetAsync(string chave)
        {
            // (Conforme o ERD do schema: transferencia)
            string sql = @"
                SELECT 
                    chave_idempotencia AS ChaveIdempotencia,
                    requisicao AS Requisicao,
                    resultado AS Resultado
                FROM idempotencia 
                WHERE chave_idempotencia = @Chave";

            return await _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { Chave = chave });
        }

        public async Task SalvarAsync(Idempotencia idempotencia)
        {
            string sql = @"
                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado);
            ";
            await _dbConnection.ExecuteAsync(sql, idempotencia);
        }
    }
}