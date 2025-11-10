using System;

namespace MyBancoApi.ContaCorrente.Application.Exceptions
{
    // Exceção customizada para lidar com falhas de validação de negócio
    public class CustomValidationException : Exception
    {
        public string TipoFalha { get; }

        public CustomValidationException(string message, string tipoFalha)
            : base(message)
        {
            TipoFalha = tipoFalha;
        }
    }
}