using MediatR;
using System.Text.Json.Serialization;

namespace MyBancoApi.ContaCorrente.Application.Queries.ConsultarSaldo
{
    // O Query (CQRS) que representa a intenção de consultar o saldo
    // Ele retorna o DTO de resposta 'ConsultarSaldoResponse'
    public class ConsultarSaldoQuery : IRequest<ConsultarSaldoResponse>
    {
        // Este campo será injetado pelo Program.cs a partir do Token JWT
        [JsonIgnore]
        public string IdContaCorrenteLogada { get; set; }
    }
}