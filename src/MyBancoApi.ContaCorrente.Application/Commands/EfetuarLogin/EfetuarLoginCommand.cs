using MediatR;

namespace MyBancoApi.ContaCorrente.Application.Commands.EfetuarLogin
{
    // O Comando (DTO de entrada) para o Login
    public class EfetuarLoginCommand : IRequest<EfetuarLoginResponse>
    {
        // Requisito: "Receber o número da conta ou o CPF do usuário"
        public string ContaOuCpf { get; set; }
        public string Senha { get; set; }
    }
}