using MyBancoApi.Transferencia.Application.DTOs;
using MyBancoApi.Transferencia.Application.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MyBancoApi.Transferencia.Infrastructure.Clients
{
    // Esta é a implementação do Cliente HTTP.
    // É responsável por chamar a API Conta Corrente.
    public class ContaCorrenteApiClient : IContaCorrenteApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContaCorrenteApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SolicitarMovimentacaoAsync(MovimentacaoRequest request, string token)
        {
            // 1. Cria um cliente HTTP nomeado (será configurado no Program.cs)
            var client = _httpClientFactory.CreateClient("ContaCorrenteApi");

            // 2. Repassa o token JWT do usuário logado
            // Requisito: "Repassar o token"
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 3. Chama o endpoint de movimentação da outra API
            // (O endereço base, ex: "http://localhost:5001", estará no Program.cs)
            var response = await client.PostAsJsonAsync("api/contas/movimentacao", request);

            // 4. Retorna 'true' se a API Conta Corrente retornou 2xx (sucesso)
            // e 'false' se retornou 4xx ou 5xx (falha)
            return response.IsSuccessStatusCode;
        }
    }
}