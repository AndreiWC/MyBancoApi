using MyBancoApi.Transferencia.Application.Messages;
using System.Threading.Tasks;

namespace MyBancoApi.Transferencia.Application.Interfaces
{
    // Interface (Contrato) para o Produtor do Kafka
    public interface ITransferenciaProducer
    {
        Task ProduzirTransferenciaRealizadaAsync(TransferenciaRealizadaMessage message);
    }
}