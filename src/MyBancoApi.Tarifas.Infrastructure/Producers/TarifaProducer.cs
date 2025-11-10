using KafkaFlow;
using KafkaFlow.Producers;
using MyBancoApi.Tarifas.Application.Interfaces;
using MyBancoApi.Tarifas.Application.Messages;
using System.Threading.Tasks;

namespace MyBancoApi.Tarifas.Infrastructure.Producers
{
    public class TarifaProducer : ITarifaProducer
    {
        private readonly IProducerAccessor _producerAccessor;

        public TarifaProducer(IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor;
        }

        public async Task ProduzirTarifaRealizadaAsync(TarifaRealizadaMessage message)
        {
            var producer = _producerAccessor.GetProducer("tarifas-producer");

            await producer.ProduceAsync(
                "tarifas_realizadas", // Nome do Tópico
                message.IdContaCorrente, // Chave da mensagem (para particionamento)
                message
            );
        }
    }
}