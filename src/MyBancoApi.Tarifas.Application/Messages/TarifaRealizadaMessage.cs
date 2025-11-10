namespace MyBancoApi.Tarifas.Application.Messages
{
    // DTO para a mensagem enviada ao Tópico 2
    // (Consumida pela API Conta Corrente)
    public class TarifaRealizadaMessage
    {
        public string IdContaCorrente { get; set; }
        public decimal ValorTarifa { get; set; }
    }
}