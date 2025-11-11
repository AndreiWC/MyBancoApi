using KafkaFlow;
using KafkaFlow.Producers;
using MyBancoApi.Transferencia.Application.Interfaces;
using MyBancoApi.Transferencia.Application.Messages;
using System.Threading.Tasks;

namespace MyBancoApi.Transferencia.Infrastructure.Producers
{
    // Implementação do Produtor
    public class TransferenciaProducer : ITransferenciaProducer
    {
        private readonly IProducerAccessor _producerAccessor;

        public TransferenciaProducer(IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor;
        }

        public async Task ProduzirTransferenciaRealizadaAsync(TransferenciaRealizadaMessage message)
        {
            var producer = _producerAccessor.GetProducer("transferencias-producer");

            await producer.ProduceAsync(
                "transferencias_realizadas", // Nome do Tópico 1
                message.IdContaCorrenteOrigem, // Chave da mensagem
                message
            );
        }
    }
}