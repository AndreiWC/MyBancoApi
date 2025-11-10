using MyBancoApi.ContaCorrente.Domain.Entities;

namespace MyBancoApi.ContaCorrente.Domain.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia> GetAsync(string chave);
        Task SalvarAsync(Idempotencia idempotencia);
    }
}