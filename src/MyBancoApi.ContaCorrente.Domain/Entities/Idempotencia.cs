using System.Text.Json;

namespace MyBancoApi.ContaCorrente.Domain.Entities
{
    // Entidade baseada no ERD 'idempotencia'
    public class Idempotencia
    {
        public string ChaveIdempotencia { get; private set; }
        public string Requisicao { get; private set; } // JSON da Requisição
        public string Resultado { get; private set; } // JSON da Resposta

        private Idempotencia() { }

        public Idempotencia(string chave, object requisicao, object resultado)
        {
            ChaveIdempotencia = chave;
            Requisicao = JsonSerializer.Serialize(requisicao);
            Resultado = JsonSerializer.Serialize(resultado);
        }
    }
}