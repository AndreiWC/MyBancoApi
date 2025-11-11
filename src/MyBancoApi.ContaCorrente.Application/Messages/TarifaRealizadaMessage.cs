namespace MyBancoApi.ContaCorrente.Application.Messages
{
    // DTO (Mensagem) que será recebida do Tópico 2
    // (Produzida pelo Tarifas.Worker)
    // Esta classe deve ser idêntica à do projeto Tarifas.Application
    public class TarifaRealizadaMessage
    {
        public string IdContaCorrente { get; set; }
        public decimal ValorTarifa { get; set; }
    }
}