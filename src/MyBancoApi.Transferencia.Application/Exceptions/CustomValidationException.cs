using System;

namespace MyBancoApi.Transferencia.Application.Exceptions
{
    // Exceção customizada para lidar com falhas de validação de negócio
    // (Idêntica à do outro microsserviço)
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