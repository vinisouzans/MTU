using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Locacao;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly AppDbContext _context;

        public LocacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LocacaoResponseDTO> CriarLocacaoAsync(LocacaoCreateDTO dto)
        {
            var entregador = await _context.Entregadores.FindAsync(dto.EntregadorId);
            if (entregador == null) throw new InvalidOperationException("Entregador não encontrado");
            if (!Locacao.EntregadorPodeAlugar(entregador.TipoCNH))
                throw new InvalidOperationException("Entregador não possui CNH categoria A");

            var moto = await _context.Motos.FindAsync(dto.MotoId);
            if (moto == null) throw new InvalidOperationException("Moto não encontrada");

            // Define a data de início como o dia seguinte à data de cadastro
            DateTime dataInicio = DateTime.UtcNow.AddDays(1);
            DateTime dataTermino = dto.DataTermino;

            // Verifica se a moto já está locada nesse período
            bool motoIndisponivel = await _context.Locacoes.AnyAsync(l =>
                l.MotoId == dto.MotoId &&
                l.DataInicio <= dataTermino &&
                l.DataTermino >= dataInicio
            );

            if (motoIndisponivel)
                throw new InvalidOperationException("A moto já está locada neste período.");

            var locacao = new Locacao
            {
                EntregadorId = dto.EntregadorId,
                MotoId = dto.MotoId,
                Plano = dto.Plano,
                DataPrevistaTermino = dataTermino,
                DataTermino = dataTermino
            };
            locacao.DefinirDataInicio();

            _context.Locacoes.Add(locacao);
            await _context.SaveChangesAsync();

            return new LocacaoResponseDTO
            {
                Id = locacao.Id,
                EntregadorId = locacao.EntregadorId,
                MotoId = locacao.MotoId,
                DataInicio = locacao.DataInicio,
                DataTermino = locacao.DataTermino,
                DataPrevistaTermino = locacao.DataPrevistaTermino,
                ValorTotal = locacao.CalcularValorTotal()
            };
        }

        public async Task<LocacaoResponseDTO> ObterLocacaoAsync(Guid id)
        {
            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao == null) throw new InvalidOperationException("Locação não encontrada");

            return new LocacaoResponseDTO
            {
                Id = locacao.Id,
                EntregadorId = locacao.EntregadorId,
                MotoId = locacao.MotoId,
                DataInicio = locacao.DataInicio,
                DataTermino = locacao.DataTermino,
                DataPrevistaTermino = locacao.DataPrevistaTermino,
                ValorTotal = locacao.CalcularValorTotal()
            };
        }

        public async Task<SimularDevolucaoResponse> SimularDevolucaoAsync(SimularDevolucaoRequest request)
        {
            var locacao = await _context.Locacoes.FindAsync(request.LocacaoId);
            if (locacao == null) throw new InvalidOperationException("Locação não encontrada");

            decimal valor = locacao.CalcularValorTotalParaSimulacao(request.NovaDataDevolucao);

            return new SimularDevolucaoResponse
            {
                LocacaoId = locacao.Id,
                DataInicio = locacao.DataInicio,
                DataPrevistaTermino = locacao.DataPrevistaTermino,
                DataInformada = request.NovaDataDevolucao,
                ValorTotal = valor,
                Observacao = request.NovaDataDevolucao < locacao.DataPrevistaTermino
                    ? "Devolução antecipada"
                    : request.NovaDataDevolucao > locacao.DataPrevistaTermino
                        ? "Devolução atrasada"
                        : "Devolução no prazo"
            };
        }

        public async Task<List<object>> ConsultarPorEntregadorAsync(string? cnpj, string? numeroCnh)
        {
            if (string.IsNullOrEmpty(cnpj) && string.IsNullOrEmpty(numeroCnh))
                throw new InvalidOperationException("Informe o CNPJ ou o número da CNH.");

            var query = from locacao in _context.Locacoes
                        join entregador in _context.Entregadores on locacao.EntregadorId equals entregador.Id
                        join moto in _context.Motos on locacao.MotoId equals moto.Id
                        select new
                        {
                            locacao.Id,
                            locacao.DataInicio,
                            locacao.DataPrevistaTermino,
                            locacao.DataTermino,
                            locacao.Plano,
                            Entregador = entregador.Nome,
                            entregador.Cnpj,
                            entregador.NumeroCNH,
                            Moto = moto.Modelo,
                            moto.Placa
                        };

            if (!string.IsNullOrEmpty(cnpj))
                query = query.Where(x => x.Cnpj == cnpj);
            else if (!string.IsNullOrEmpty(numeroCnh))
                query = query.Where(x => x.NumeroCNH == numeroCnh);

            return await query.ToListAsync<object>();
        }
    }
}
