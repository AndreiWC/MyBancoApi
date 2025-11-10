namespace MyBancoApi.ContaCorrente.Application.Helpers
{
    // (Esta é uma simulação de validação. Não use em produção)
    public static class CpfValidator
    {
        public static bool IsValid(string cpf)
        {
            // Requisito: Validar CPF
            if (string.IsNullOrEmpty(cpf))
                return false;
            
            // Simulação simples de validação (deve ser substituída por um algoritmo real)
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            return true;
        }
    }
}