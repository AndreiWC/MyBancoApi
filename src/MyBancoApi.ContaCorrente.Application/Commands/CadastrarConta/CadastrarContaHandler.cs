using MediatR;
using MyBancoApi.ContaCorrente.Application.Helpers;
using MyBancoApi.ContaCorrente.Application.Responses;
using MyBancoApi.ContaCorrente.Domain.Entities;
using MyBancoApi.ContaCorrente.Domain.Interfaces;

namespace MyBancoApi.ContaCorrente.Application.Commands.CadastrarConta
{
    // O "Handler" (CQRS) - executa a lógica do comando
    public class CadastrarContaHandler : IRequestHandler<CadastrarContaCommand, CadastroContaResponse>
    {
        private readonly IContaCorrenteRepository _repository;

        public CadastrarContaHandler(IContaCorrenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<CadastroContaResponse> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar CPF
            if (!CpfValidator.IsValid(request.Cpf))
            {
                return new CadastroContaResponse
                {
                    Sucesso = false,
                    Mensagem = "CPF inválido.",
                    TipoFalha = "INVALID_DOCUMENT" // Requisito do documento
                };
            }

            // 2. Verificar se CPF já existe
            if (await _repository.CpfJaExisteAsync(request.Cpf))
            {
                return new CadastroContaResponse
                {
                    Sucesso = false,
                    Mensagem = "CPF já cadastrado.",
                    TipoFalha = "DOCUMENT_ALREADY_EXISTS"
                };
            }

            // 3. Criptografar senha
            var salt = SenhaHelper.GerarSalt();
            var senhaHash = SenhaHelper.Hash(request.Senha, salt);

            // 4. Criar a entidade de domínio
            var novaConta = Domain.Entities.ContaCorrente.Criar(request.Nome, request.Cpf, senhaHash, salt);
            
            // 5. Persistir no banco
            var idConta = await _repository.CriarAsync(novaConta);

            // 6. Retornar resposta de sucesso
            return new CadastroContaResponse
            {
                Sucesso = true,
                IdContaCorrente = idConta,
                NumeroConta = novaConta.Numero
            };
        }
    }
}