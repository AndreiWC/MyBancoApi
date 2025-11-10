using System;

namespace MyBancoApi.ContaCorrente.Application.Queries.ConsultarSaldo
{
    // O DTO (Response) que bate exatamente com os requisitos do documento
    public class ConsultarSaldoResponse
    {
        public string NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; }
        public decimal SaldoAtual { get; set; }
    }
}