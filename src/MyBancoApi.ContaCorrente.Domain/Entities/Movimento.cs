namespace MyBancoApi.ContaCorrente.Domain.Entities
{
    // Entidade baseada no ERD 'movimento'
    public class Movimento
    {
        public string IdMovimento { get; private set; }
        public string IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public char TipoMovimento { get; private set; } // 'C' = Crédito, 'D' = Débito
        public decimal Valor { get; private set; }

        // Construtor privado para o Dapper
        private Movimento() { }

        // Método de fábrica para criar um movimento válido
        public static Movimento Criar(string idContaCorrente, char tipoMovimento, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor deve ser positivo.");
            
            if (tipoMovimento != 'C' && tipoMovimento != 'D')
                throw new ArgumentException("Tipo de movimento inválido.");

            return new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = idContaCorrente,
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = tipoMovimento,
                Valor = valor
            };
        }
    }
}