using Dapper;
using Microsoft.Data.Sqlite;
using MyBancoApi.Tarifas.Domain.Entities;
using MyBancoApi.Tarifas.Domain.Interfaces;
using System.Threading.Tasks;

namespace MyBancoApi.Tarifas.Infrastructure.Repositories
{
    // Usando o alias para evitar conflito de namespace (CS0118)
    using TarifaEntidade = MyBancoApi.Tarifas.Domain.Entities.Tarifa;

    public class TarifaRepository : ITarifaRepository
    {
        private readonly SqliteConnection _dbConnection;

        public TarifaRepository(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task SalvarAsync(TarifaEntidade tarifa)
        {
            string sql = @"
                INSERT INTO tarifas (idtarifa, idcontacorrente, datamovimento, valor)
                VALUES (@IdTarifa, @IdContaCorrente, @DataMovimento, @Valor);
            ";
            await _dbConnection.ExecuteAsync(sql, tarifa);
        }
    }
}