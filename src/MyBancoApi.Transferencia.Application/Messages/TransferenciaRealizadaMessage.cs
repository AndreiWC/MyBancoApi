namespace MyBancoApi.Transferencia.Application.Messages
{
    // DTO (Mensagem) que será enviado para o Tópico 1
    // (Este DTO é uma "cópia" do que o Tarifas.Worker espera)
    public class TransferenciaRealizadaMessage
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrenteOrigem { get; set; }
    }
}