namespace MyBancoApi.Transferencia.Application.DTOs
{
    // Este DTO (Data Transfer Object) representa o JSON que
    // a API de Transferência vai ENVIAR para a API Conta Corrente.
    public class MovimentacaoRequest
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}