using MediatR;
using System.Text.Json.Serialization;

namespace MyBancoApi.Transferencia.Application.Commands.EfetuarTransferencia
{
    // O Comando (CQRS) que inicia o processo de transferência
    public class EfetuarTransferenciaCommand : IRequest<bool>
    {
        // Requisito: "Receber a identificação da requisição"
        public string IdRequisicao { get; set; }

        // Requisito: "o número da conta de destino"
        public string IdContaDestino { get; set; }

        // Requisito: "e o valor a ser transferido"
        public decimal Valor { get; set; }

        // Campos injetados internamente (não vêm do JSON)
        [JsonIgnore]
        public string IdContaCorrenteLogada { get; set; } // Vem do Token

        [JsonIgnore]
        public string Token { get; set; } // O Token JWT completo
    }
}