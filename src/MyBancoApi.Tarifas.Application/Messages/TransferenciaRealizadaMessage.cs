namespace MyBancoApi.Tarifas.Application.Messages
{
    // DTO para a mensagem recebida do Tópico 1
    // (Produzida pela API Transferencia)
    public class TransferenciaRealizadaMessage
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrenteOrigem { get; set; }
    }
}