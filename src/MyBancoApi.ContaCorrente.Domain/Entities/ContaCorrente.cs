namespace MyBancoApi.ContaCorrente.Domain.Entities
{
    // Esta é a nossa Entidade e Raiz de Agregação (DDD)
    public class ContaCorrente
    {
        // O IdContaCorrente será um GUID para evitar que dados
       // [cite_start]// sensíveis (CPF/Conta) transitem [cite: 15]
        public string IdContaCorrente { get; private set; }
        public string Numero { get; private set; }
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public string SenhaHash { get; private set; }
        public string Salt { get; private set; }

        // Construtor privado para o Dapper/EF
        private ContaCorrente() { }

        // Método de fábrica (Factory Method) para criar uma conta válida
        public static ContaCorrente Criar(string nome, string cpf, string senhaHash, string salt)
        {
          //  [cite_start]// (Aqui entraria a validação do CPF [cite: 30])
            if (string.IsNullOrEmpty(nome))
                throw new ArgumentException("Nome não pode ser nulo.");
            
            // Lógica para gerar um novo número de conta
            string novoNumeroConta = new Random().Next(10000, 99999).ToString();

            return new ContaCorrente
            {
                IdContaCorrente = Guid.NewGuid().ToString(),
                Numero = novoNumeroConta,
                Nome = nome, // O CPF não deve ser armazenado aqui, apenas usado no cadastro.
                Ativo = true,
                SenhaHash = senhaHash,
                Salt = salt
            };
        }

        // Comportamento do Domínio
        public void InativarConta(string senhaHashFornecida)
        {
        //    [cite_start]// Validação de senha [cite: 45]
            if (this.SenhaHash != senhaHashFornecida)
                throw new InvalidOperationException("Senha inválida.");

            if (!this.Ativo)
                throw new InvalidOperationException("Conta já está inativa.");

            this.Ativo = false; // [cite: 46]
        }
    }
}