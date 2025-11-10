using MediatR;
using MyBancoApi.ContaCorrente.Domain.Entities;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
// Alias para a entidade de Conta Corrente
using ContaCorrenteEntidade = MyBancoApi.ContaCorrente.Domain.Entities.ContaCorrente;

namespace MyBancoApi.ContaCorrente.Application.Commands.Movimentacao
{
    public class MovimentacaoHandler : IRequestHandler<MovimentacaoCommand, MovimentacaoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IMovimentoRepository _movimentoRepo;
        private readonly IIdempotenciaRepository _idempotenciaRepo;

        public MovimentacaoHandler(IContaCorrenteRepository contaRepo, IMovimentoRepository movimentoRepo, IIdempotenciaRepository idempotenciaRepo)
        {
            _contaRepo = contaRepo;
            _movimentoRepo = movimentoRepo;
            _idempotenciaRepo = idempotenciaRepo;
        }

        public async Task<MovimentacaoResponse> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
        {
            // 1. CHECAGEM DE IDEMPOTÊNCIA
            // Requisito do Time de Crédito: ser idempotente
            var requisicaoSalva = await _idempotenciaRepo.GetAsync(request.IdRequisicao);
            if (requisicaoSalva != null)
            {
                // Requisição já processada, retorna sucesso (HTTP 204)
                return new MovimentacaoResponse { Sucesso = true, StatusCode = 204 };
            }

            // 2. VALIDAÇÕES
            
            // Requisito: "caso não receba esse campo [IdContaCorrente], deve utilizar a identificação da conta que está no token"
            string idContaAlvo = request.IdContaCorrente ?? request.IdContaCorrenteLogada;

            var conta = await _contaRepo.GetByIdAsync(idContaAlvo);

            // Requisito: "Apenas contas correntes cadastradas podem receber movimentação"
            if (conta == null)
                return Falha("Conta não encontrada.", "INVALID_ACCOUNT");

            // Requisito: "Apenas contas correntes ativas podem receber movimentação"
            if (!conta.Ativo)
                return Falha("Conta inativa.", "INACTIVE_ACCOUNT");

            // Requisito: "Apenas valores positivos podem ser recebidos"
            if (request.Valor <= 0)
                return Falha("Valor deve ser positivo.", "INVALID_VALUE");

            // Requisito: "Apenas os tipos “débito” ou “crédito” podem ser aceitos"
            if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
                return Falha("Tipo de movimento inválido. Use 'C' ou 'D'.", "INVALID_TYPE");

            // Requisito: "Apenas o tipo “crédito” pode ser aceito caso o número da conta seja diferente do usuário logado"
            // (Em outras palavras: você não pode DEBITAR da conta de outra pessoa)
            if (idContaAlvo != request.IdContaCorrenteLogada && request.TipoMovimento == 'D')
                return Falha("Não é permitido realizar débitos na conta de terceiros.", "INVALID_TYPE");

            // 3. EXECUÇÃO
            
            var movimento = Movimento.Criar(idContaAlvo, request.TipoMovimento, request.Valor);
            
            // (Em um cenário real, usaríamos uma Transação de banco de dados aqui
            // para salvar o Movimento e a Idempotência atomicamente)

            // Salva o movimento
            await _movimentoRepo.SalvarAsync(movimento);

            // Salva o registro de idempotência
            var resultadoSucesso = new { StatusCode = 204, Message = "Movimentação realizada." };
            await _idempotenciaRepo.SalvarAsync(new Idempotencia(request.IdRequisicao, request, resultadoSucesso));

            // Retorna sucesso
            return new MovimentacaoResponse { Sucesso = true, StatusCode = 204 };
        }

        private MovimentacaoResponse Falha(string msg, string tipo)
        {
            return new MovimentacaoResponse
            {
                Sucesso = false,
                Mensagem = msg,
                TipoFalha = tipo,
                StatusCode = 400 // HTTP 400
            };
        }
    }
}