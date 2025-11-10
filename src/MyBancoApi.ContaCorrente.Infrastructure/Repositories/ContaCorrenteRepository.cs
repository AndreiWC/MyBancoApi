using Dapper;
using Microsoft.Data.Sqlite;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
// 1. Este é o alias que resolve o conflito de namespace
using ContaCorrenteEntidade = MyBancoApi.ContaCorrente.Domain.Entities.ContaCorrente;


namespace MyBancoApi.ContaCorrente.Infrastructure.Repositories
{
    // A classe implementa a interface
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly SqliteConnection _dbConnection;

        public ContaCorrenteRepository(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // 2. O método usa o alias 'ContaCorrenteEntidade'
        public async Task<string> CriarAsync(ContaCorrenteEntidade conta)
        {
            string sql = @"
                INSERT INTO contacorrente (idcontacorrente, numero, nome, ativo, senhaHash, salt)
                VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @SenhaHash, @Salt);
            ";
            
            await _dbConnection.ExecuteAsync(sql, conta);
            return conta.IdContaCorrente;
        }

        public async Task<bool> CpfJaExisteAsync(string cpf)
        {
            // Simulação (a lógica real necessitaria de uma tabela 'Cliente')
            return await Task.FromResult(false);
        }

        // 3. O método usa o alias 'ContaCorrenteEntidade' no retorno
        public async Task<ContaCorrenteEntidade> GetByCpfOuContaAsync(string cpfOuConta)
        {
            // O requisito pede para buscar por CPF ou Conta.
            // Nossa simulação de CPF (sem tabela Cliente) permite buscar apenas por CONTA.
            string sql = @"
                SELECT 
                    idcontacorrente as IdContaCorrente,
                    numero as Numero,
                    nome as Nome,
                    ativo as Ativo,
                    senhaHash as SenhaHash,
                    salt as Salt
                FROM contacorrente 
                WHERE numero = @CpfOuConta";
            
            // 4. E no 'Generics' do Dapper
            return await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrenteEntidade>(sql, new { CpfOuConta = cpfOuConta });
        }

        // 5. O método usa o alias 'ContaCorrenteEntidade' no retorno
        public async Task<ContaCorrenteEntidade> GetByIdAsync(string idContaCorrente)
        {
            string sql = @"
                SELECT 
                    idcontacorrente as IdContaCorrente,
                    numero as Numero,
                    nome as Nome,
                    ativo as Ativo,
                    senhaHash as SenhaHash,
                    salt as Salt
                FROM contacorrente 
                WHERE idcontacorrente = @Id";
                
            // 6. E no 'Generics' do Dapper
            return await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrenteEntidade>(sql, new { Id = idContaCorrente });
        }
    }
}