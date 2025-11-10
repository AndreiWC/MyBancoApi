using System.Text.Json;

namespace MyBancoApi.Transferencia.Domain.Entities
{
    // Entidade baseada no ERD 'idempotencia' (Schema: transferencia)
    // É idêntica à do outro microsserviço, mas pertence a este domínio.
    public class Idempotencia
    {
        public string ChaveIdempotencia { get; private set; }
        public string Requisicao { get; private set; } // JSON da Requisição
        public string Resultado { get; private set; } // JSON da Resposta

        private Idempotencia() { }

        // Construtor usado pelo Handler
        public Idempotencia(string chave, object requisicao, object resultado)
        {
            ChaveIdempotencia = chave;
            Requisicao = JsonSerializer.Serialize(requisicao);
            Resultado = JsonSerializer.Serialize(resultado);
        }
    }
}