using MyBancoApi.Transferencia.Domain.Entities;

namespace MyBancoApi.Transferencia.Domain.Interfaces
{
    // Interface do Repositório (Contrato do Domínio)
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia> GetAsync(string chave);
        Task SalvarAsync(Idempotencia idempotencia);
    }
}