using MediatR;
using MyBancoApi.ContaCorrente.Application.Exceptions;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyBancoApi.ContaCorrente.Application.Queries.ConsultarSaldo
{
    // O Handler (CQRS) que executa a lógica da consulta
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, ConsultarSaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepo;
        private readonly IMovimentoRepository _movimentoRepo;

        public ConsultarSaldoHandler(IContaCorrenteRepository contaRepo, IMovimentoRepository movimentoRepo)
        {
            _contaRepo = contaRepo;
            _movimentoRepo = movimentoRepo;
        }

        public async Task<ConsultarSaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            // 1. Busca os dados da conta
            var conta = await _contaRepo.GetByIdAsync(request.IdContaCorrenteLogada);

            // 2. Validações (Requisitos)
            // Requisito: "Apenas contas correntes cadastradas podem consultar o saldo"
            if (conta == null)
                throw new CustomValidationException("Conta não encontrada.", "INVALID_ACCOUNT");

            // Requisito: "Apenas contas correntes ativas podem consultar o saldo"
            if (!conta.Ativo)
                throw new CustomValidationException("Conta inativa.", "INACTIVE_ACCOUNT");

            // 3. Cálculo do Saldo
            decimal saldo = await _movimentoRepo.GetSaldoAsync(request.IdContaCorrenteLogada);

            // 4. Montagem da Resposta (DTO)
            // Requisitos: Retornar Número da conta, Nome do titular, Data e hora, Saldo
            return new ConsultarSaldoResponse
            {
                NumeroConta = conta.Numero,
                NomeTitular = conta.Nome,
                DataConsulta = DateTime.UtcNow,
                SaldoAtual = saldo
            };
        }
    }
}