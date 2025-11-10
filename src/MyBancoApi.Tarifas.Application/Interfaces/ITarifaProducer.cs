using MyBancoApi.Tarifas.Application.Messages;

namespace MyBancoApi.Tarifas.Application.Interfaces
{
    // Interface para o produtor do Tópico 2
    public interface ITarifaProducer
    {
        Task ProduzirTarifaRealizadaAsync(TarifaRealizadaMessage message);
    }
}