using MyBancoApi.Tarifas.Domain.Entities;

namespace MyBancoApi.Tarifas.Domain.Interfaces
{
    // Contrato para o repositório de tarifas
    public interface ITarifaRepository
    {
        Task SalvarAsync(Tarifa tarifa);
    }
}