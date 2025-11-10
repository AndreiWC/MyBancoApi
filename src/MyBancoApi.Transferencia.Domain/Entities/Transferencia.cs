using System;

namespace MyBancoApi.Transferencia.Domain.Entities
{
    // Entidade baseada no ERD 'transferencia'
    public class Transferencia
    {
        public string IdTransferencia { get; private set; }
        public string IdContaCorrenteOrigem { get; private set; }
        public string IdContaCorrenteDestino { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        private Transferencia() { }

        // Método de fábrica (Factory Method)
        public static Transferencia Criar(string idOrigem, string idDestino, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor da transferência deve ser positivo.");

            if (string.IsNullOrEmpty(idOrigem) || string.IsNullOrEmpty(idDestino))
                throw new ArgumentException("Contas de origem e destino são obrigatórias.");

            return new Transferencia
            {
                IdTransferencia = Guid.NewGuid().ToString(),
                IdContaCorrenteOrigem = idOrigem,
                IdContaCorrenteDestino = idDestino,
                DataMovimento = DateTime.UtcNow,
                Valor = valor
            };
        }
    }
}