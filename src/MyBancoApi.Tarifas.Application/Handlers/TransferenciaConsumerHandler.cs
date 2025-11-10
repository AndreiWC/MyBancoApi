using KafkaFlow;
using MyBancoApi.Tarifas.Application.Interfaces;
using MyBancoApi.Tarifas.Application.Messages;
using MyBancoApi.Tarifas.Domain.Entities;
using MyBancoApi.Tarifas.Domain.Interfaces;
using Microsoft.Extensions.Configuration; // Para ler o valor da tarifa

namespace MyBancoApi.Tarifas.Application.Handlers
{
    // O "Cérebro" do Worker: consome Tópico 1, salva no DB, produz Tópico 2
    // Diga ao C# para usar a interface do namespace KafkaFlow
    public class TransferenciaConsumerHandler : IMessageHandler<TransferenciaRealizadaMessage>
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ITarifaProducer _tarifaProducer;
        private readonly decimal _valorTarifa;

        public TransferenciaConsumerHandler(
            ITarifaRepository tarifaRepository,
            ITarifaProducer tarifaProducer,
            IConfiguration configuration)
        {
            _tarifaRepository = tarifaRepository;
            _tarifaProducer = tarifaProducer;

            // Requisito: "Ter parametrizado no arquivo 'appsettings.json' o valor da tarifa"
            _valorTarifa = configuration.GetValue<decimal>("Configuracoes:ValorTarifa");
        }


        public async Task Handle(IMessageContext context, TransferenciaRealizadaMessage message)
        {
            // 1. Criar a entidade Tarifa
            var novaTarifa = Tarifa.Criar(message.IdContaCorrenteOrigem, _valorTarifa);

            // 2. Persistir no banco de dados
            await _tarifaRepository.SalvarAsync(novaTarifa);

            // 3. Preparar a mensagem para o próximo tópico
            var mensagemTarifaRealizada = new TarifaRealizadaMessage
            {
                IdContaCorrente = novaTarifa.IdContaCorrente,
                ValorTarifa = novaTarifa.Valor
            };

            // 4. Enviar para o tópico "tarifas_realizadas"
            await _tarifaProducer.ProduzirTarifaRealizadaAsync(mensagemTarifaRealizada);
        }
    }
}