using MyBancoApi.ContaCorrente.Domain.Entities;

namespace MyBancoApi.ContaCorrente.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task SalvarAsync(Movimento movimento);

        Task<decimal> GetSaldoAsync(string idContaCorrente);
    }
}