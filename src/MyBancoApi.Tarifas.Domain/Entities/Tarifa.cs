namespace MyBancoApi.Tarifas.Domain.Entities
{
    // Entidade que representa a tarifa cobrada
    public class Tarifa
    {
        public string IdTarifa { get; private set; }
        public string IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public decimal Valor { get; private set; }

        // Método de fábrica
        public static Tarifa Criar(string idContaCorrente, decimal valor)
        {
            return new Tarifa
            {
                IdTarifa = Guid.NewGuid().ToString(),
                IdContaCorrente = idContaCorrente,
                DataMovimento = DateTime.UtcNow,
                Valor = valor
            };
        }
    }
}