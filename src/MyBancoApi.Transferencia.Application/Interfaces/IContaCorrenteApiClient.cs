using MyBancoApi.Transferencia.Application.DTOs;
using System.Threading.Tasks;

namespace MyBancoApi.Transferencia.Application.Interfaces
{
    // Interface (Contrato) para o Cliente HTTP
    // Define como a API de Transferência se comunica com a API Conta Corrente
    public interface IContaCorrenteApiClient
    {
        // Requisito: "Realizar chamada para a API Conta Corrente para realizar um débito/crédito"
        // Retorna 'true' se a movimentação foi bem-sucedida (HTTP 204),
        // e 'false' se falhou (HTTP 400 ou 500).
        Task<bool> SolicitarMovimentacaoAsync(MovimentacaoRequest request, string token);
    }
}