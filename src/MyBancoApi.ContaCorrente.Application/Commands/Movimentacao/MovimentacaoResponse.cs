namespace MyBancoApi.ContaCorrente.Application.Commands.Movimentacao
{
    // DTO para sinalizar sucesso ou falha na validação
    public class MovimentacaoResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string TipoFalha { get; set; }
        
        // Para a lógica de idempotência, guardamos o status code
        public int StatusCode { get; set; } 
    }
}