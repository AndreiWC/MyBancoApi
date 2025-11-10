using MediatR;
using MyBancoApi.ContaCorrente.Application.Helpers;
using MyBancoApi.ContaCorrente.Application.Interfaces;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
// O alias para a entidade de Conta Corrente
using ContaCorrenteEntidade = MyBancoApi.ContaCorrente.Domain.Entities.ContaCorrente;

namespace MyBancoApi.ContaCorrente.Application.Commands.EfetuarLogin
{
    // O Handler (lógica) do Login
    public class EfetuarLoginHandler : IRequestHandler<EfetuarLoginCommand, EfetuarLoginResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public EfetuarLoginHandler(IContaCorrenteRepository repository, IJwtTokenGenerator jwtGenerator)
        {
            _repository = repository;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<EfetuarLoginResponse> Handle(EfetuarLoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar a conta pelo número (ou CPF, se tivéssemos)
            var conta = await _repository.GetByCpfOuContaAsync(request.ContaOuCpf);

            if (conta == null)
            {
                return new EfetuarLoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Usuário ou senha inválidos.",
                    TipoFalha = "USER_UNAUTHORIZED" // Requisito do documento
                };
            }

            // 2. Validar a senha
            var senhaHash = SenhaHelper.Hash(request.Senha, conta.Salt);
            if (senhaHash != conta.SenhaHash)
            {
                return new EfetuarLoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Usuário ou senha inválidos.",
                    TipoFalha = "USER_UNAUTHORIZED"
                };
            }

            // 3. Gerar o Token JWT
            var token = _jwtGenerator.GerarToken(conta.IdContaCorrente);

            return new EfetuarLoginResponse
            {
                Sucesso = true,
                Token = token
            };
        }
    }
}