using System.Security.Cryptography;
using System.Text;

namespace MyBancoApi.ContaCorrente.Application.Helpers
{
    // (Esta é uma simulação de criptografia. Use BCrypt ou PBKDF2 em produção)
    public static class SenhaHelper
    {
        public static string GerarSalt()
        {
            // Simulação de Salt
            return "ABC123XYZ"; 
        }

        public static string Hash(string senha, string salt)
        {
            // Simulação de Hash. NÃO FAÇA ISSO EM PRODUÇÃO.
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha + salt));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool ValidarHash(string senhaFornecida, string salt, string hashArmazenado)
        {
            string hashCalculado = Hash(senhaFornecida, salt);
            return hashCalculado == hashArmazenado;
        }
    }
}