using Dapper;
using Microsoft.Data.Sqlite;
using MyBancoApi.Transferencia.Domain.Interfaces;
// 1. **CORREÇÃO DO ERRO CS0118**
// Adicionamos o alias para a entidade de Transferencia
using TransferenciaEntidade = MyBancoApi.Transferencia.Domain.Entities.Transferencia;

namespace MyBancoApi.Transferencia.Infrastructure.Repositories
{
    // Esta é a implementação do Repositório (Dapper)
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly SqliteConnection _dbConnection;

        public TransferenciaRepository(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // 2. Usamos o alias aqui
        public async Task SalvarAsync(TransferenciaEntidade transferencia)
        {
            // (Conforme o ERD do schema: transferencia)
            string sql = @"
                INSERT INTO transferencia (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
                VALUES (@IdTransferencia, @IdContaCorrenteOrigem, @IdContaCorrenteDestino, @DataMovimento, @Valor);
            ";

            await _dbConnection.ExecuteAsync(sql, transferencia);
        }
    }
}