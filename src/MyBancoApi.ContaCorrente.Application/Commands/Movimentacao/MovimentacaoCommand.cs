using MediatR;
using System.Text.Json.Serialization;

namespace MyBancoApi.ContaCorrente.Application.Commands.Movimentacao
{
    public class MovimentacaoCommand : IRequest<MovimentacaoResponse>
    {
        // Requisito: "Receber a identificação da requisição"
        public string IdRequisicao { get; set; }

        // Requisito: "O número da conta corrente é opcional"
        public string IdContaCorrente { get; set; }

        public decimal Valor { get; set; }

        // Requisito: "tipo de movimento (C = Crédito, D = Débito)"
        public char TipoMovimento { get; set; }

        // Este campo NÃO VEM do request body, será preenchido
        // internamente com o ID do token do usuário logado.
        [JsonIgnore]
        public string IdContaCorrenteLogada { get; set; }
    }
}