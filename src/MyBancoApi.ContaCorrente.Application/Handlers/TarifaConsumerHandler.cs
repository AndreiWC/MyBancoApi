using KafkaFlow;
using MyBancoApi.ContaCorrente.Application.Messages;
using MyBancoApi.ContaCorrente.Domain.Entities;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
using System.Threading.Tasks;
// Usando o alias para a entidade
using ContaCorrenteEntidade = MyBancoApi.ContaCorrente.Domain.Entities.ContaCorrente;

namespace MyBancoApi.ContaCorrente.Application.Handlers
{
    // Handler que consome o Tópico 2 (tarifas_realizadas)
    public class TarifaConsumerHandler : IMessageHandler<TarifaRealizadaMessage>
    {
        private readonly IMovimentoRepository _movimentoRepo;
        private readonly IContaCorrenteRepository _contaRepo;

        // (Nota: Em um cenário real, adicionaríamos ILogger para registrar falhas)

        public TarifaConsumerHandler(IMovimentoRepository movimentoRepo, IContaCorrenteRepository contaRepo)
        {
            _movimentoRepo = movimentoRepo;
            _contaRepo = contaRepo;
        }

        // Usamos Handle (sem Async no nome) para bater com a correção que fizemos
        public async Task Handle(IMessageContext context, TarifaRealizadaMessage message)
        {
            // Requisito: "implementando o mesmo funcionamento do serviço movimentação"
            // (Sempre debitando o valor tarifado)

            // 1. Validação (simples, pois é um serviço interno)
            var conta = await _contaRepo.GetByIdAsync(message.IdContaCorrente);

            // Não debita se a conta não existir ou estiver inativa
            if (conta == null || !conta.Ativo)
            {
                // (Logar o erro aqui)
                return;
            }

            // 2. Criar o Movimento de Débito da Tarifa
            var movimentoDebito = Movimento.Criar(
                message.IdContaCorrente,
                'D', // Débito
                message.ValorTarifa
            );

            // 3. Salvar no banco
            await _movimentoRepo.SalvarAsync(movimentoDebito);

            // (Se falhar, o Kafka tentará reprocessar a mensagem)
        }
    }
}