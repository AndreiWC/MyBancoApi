// 1. Apague qualquer outro 'using' para a entidade
// 2. Adicione este alias:
using ContaCorrenteEntidade = MyBancoApi.ContaCorrente.Domain.Entities.ContaCorrente;

namespace MyBancoApi.ContaCorrente.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        // 3. Use o alias 'ContaCorrenteEntidade' aqui:
        Task<ContaCorrenteEntidade> GetByIdAsync(string idContaCorrente);

        // 4. E aqui:
        Task<ContaCorrenteEntidade> GetByCpfOuContaAsync(string cpfOuConta);

        Task<bool> CpfJaExisteAsync(string cpf);
        
        // 5. E aqui:
        Task<string> CriarAsync(ContaCorrenteEntidade conta);
    }
}