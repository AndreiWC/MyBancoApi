namespace MyBancoApi.ContaCorrente.Application.Commands.EfetuarLogin
{
    // O DTO de Resposta do Login
    public class EfetuarLoginResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string TipoFalha { get; set; } // Ex: "USER_UNAUTHORIZED"
        public string Token { get; set; }
    }
}