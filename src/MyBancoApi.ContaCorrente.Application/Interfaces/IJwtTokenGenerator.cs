namespace MyBancoApi.ContaCorrente.Application.Interfaces
{
    // Interface para o serviço de geração de token
    public interface IJwtTokenGenerator
    {
        string GerarToken(string idContaCorrente);
    }
}