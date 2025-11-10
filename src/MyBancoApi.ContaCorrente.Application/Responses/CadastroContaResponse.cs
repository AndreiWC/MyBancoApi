namespace MyBancoApi.ContaCorrente.Application.Responses
{
    // Um DTO (Data Transfer Object) de resposta
    public class CadastroContaResponse
    {
        public string IdContaCorrente { get; set; }
        public string NumeroConta { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } // Para erros
        public string TipoFalha { get; set; } // Ex: "INVALID_DOCUMENT" [cite: 33]
    }
}