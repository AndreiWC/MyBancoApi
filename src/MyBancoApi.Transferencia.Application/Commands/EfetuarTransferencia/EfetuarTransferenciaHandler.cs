
using MediatR;
using MyBancoApi.Transferencia.Application.DTOs;
using MyBancoApi.Transferencia.Application.Exceptions;
using MyBancoApi.Transferencia.Application.Interfaces;
using MyBancoApi.Transferencia.Domain.Interfaces;
// 1. **CORREÇÃO PROATIVA DO ERRO CS0118**
// Adicionamos os aliases para as entidades
using TransferenciaEntidade = MyBancoApi.Transferencia.Domain.Entities.Transferencia;
using IdempotenciaEntidade = MyBancoApi.Transferencia.Domain.Entities.Idempotencia;


namespace MyBancoApi.Transferencia.Application.Commands.EfetuarTransferencia
{
    // O "Cérebro" do microsserviço de Transferência
    public class EfetuarTransferenciaHandler : IRequestHandler<EfetuarTransferenciaCommand, bool>
    {
        private readonly IIdempotenciaRepository _idempotenciaRepo;
        private readonly ITransferenciaRepository _transferenciaRepo;
        private readonly IContaCorrenteApiClient _apiContaCorrente;

        public EfetuarTransferenciaHandler(
            IIdempotenciaRepository idempotenciaRepo,
            ITransferenciaRepository transferenciaRepo,
            IContaCorrenteApiClient apiContaCorrente)
        {
            _idempotenciaRepo = idempotenciaRepo;
            _transferenciaRepo = transferenciaRepo;
            _apiContaCorrente = apiContaCorrente;
        }

        public async Task<bool> Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            // 1. CHECAGEM DE IDEMPOTÊNCIA
            var requisicaoSalva = await _idempotenciaRepo.GetAsync(request.IdRequisicao);
            if (requisicaoSalva != null)
            {
                return true; // Requisição já processada com sucesso
            }

            // 2. VALIDAÇÕES INICIAIS
            // Requisito: "Apenas valores positivos podem ser recebidos"
            if (request.Valor <= 0)
                throw new CustomValidationException("Valor deve ser positivo.", "INVALID_VALUE");

            // (As validações de "conta ativa" e "conta cadastrada"
            // serão feitas pela própria API Conta Corrente no passo 3)


            // 3. ORQUESTRAÇÃO: PASSO 1 - O DÉBITO
            // Requisito: "Realizar chamada para a API Conta Corrente para realizar um débito"
            var requisicaoDebito = new MovimentacaoRequest
            {
                IdRequisicao = $"{request.IdRequisicao}-D", // Chave única para o débito
                IdContaCorrente = request.IdContaCorrenteLogada,
                TipoMovimento = 'D',
                Valor = request.Valor
            };

            bool debitoOk = await _apiContaCorrente.SolicitarMovimentacaoAsync(requisicaoDebito, request.Token);

            if (!debitoOk)
            {
                // Se o débito falhar (Ex: saldo insuficiente, conta inativa), a transferência falha.
                throw new CustomValidationException("Falha ao debitar valor da conta de origem.", "DEBIT_FAILED");
            }

            // 4. ORQUESTRAÇÃO: PASSO 2 - O CRÉDITO
            // Requisito: "Realizar chamada para a API Conta Corrente para realizar um crédito"
            var requisicaoCredito = new MovimentacaoRequest
            {
                IdRequisicao = $"{request.IdRequisicao}-C", // Chave única para o crédito
                IdContaCorrente = request.IdContaDestino,
                TipoMovimento = 'C',
                Valor = request.Valor
            };

            bool creditoOk = await _apiContaCorrente.SolicitarMovimentacaoAsync(requisicaoCredito, request.Token);

            if (!creditoOk)
            {
                // 5. ORQUESTRAÇÃO: PASSO 3 - O ESTORNO (ROLLBACK)
                // Requisito: "Em caso de falhas deve ser feito um estorno na conta logada"

                var requisicaoEstorno = new MovimentacaoRequest
                {
                    IdRequisicao = $"{request.IdRequisicao}-E", // Chave única para o estorno
                    IdContaCorrente = request.IdContaCorrenteLogada,
                    TipoMovimento = 'C', // Estorno é um Crédito
                    Valor = request.Valor
                };

                // Tenta estornar. Se o estorno falhar, um log de erro crítico deve ser gerado.
                await _apiContaCorrente.SolicitarMovimentacaoAsync(requisicaoEstorno, request.Token);

                throw new CustomValidationException("Falha ao creditar valor na conta de destino. Estorno realizado.", "CREDIT_FAILED");
            }

            // 6. SUCESSO E PERSISTÊNCIA
            // Requisito: "Persistir na tabela TRANSFERENCIA"
            var transferencia = TransferenciaEntidade.Criar(
                request.IdContaCorrenteLogada,
                request.IdContaDestino,
                request.Valor);

            await _transferenciaRepo.SalvarAsync(transferencia);

            // Salva o registro de idempotência
            await _idempotenciaRepo.SalvarAsync(
                new IdempotenciaEntidade(request.IdRequisicao, request, new { Sucesso = true }));

            return true;
        }
    }
}