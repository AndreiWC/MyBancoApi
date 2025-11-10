using MediatR;
using MyBancoApi.ContaCorrente.Application.Responses;

namespace MyBancoApi.ContaCorrente.Application.Commands.CadastrarConta
{
    // O "Command" (CQRS) - representa a intenção de cadastrar
    // Retorna a Resposta customizada (CadastroContaResponse)
    public class CadastrarContaCommand : IRequest<CadastroContaResponse>
    {
        public string Cpf { get; set; }
        public string Nome { get; set; } // Adicionado para o modelo
        public string Senha { get; set; }
    }
}