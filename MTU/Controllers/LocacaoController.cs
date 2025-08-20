using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Locacao;
using MTU.Model;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/locacoes")]
    public class LocacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocacaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LocacaoResponseDTO>> CriarLocacao(LocacaoCreateDTO dto)
        {
            var entregador = await _context.Entregadores.FindAsync(dto.EntregadorId);
            if (entregador == null) return BadRequest("Entregador não encontrado");
            if (!Locacao.EntregadorPodeAlugar(entregador.TipoCNH))
                return BadRequest("Entregador não possui CNH categoria A");

            var moto = await _context.Motos.FindAsync(dto.MotoId);
            if (moto == null) return BadRequest("Moto não encontrada");

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
                return BadRequest("A moto já está locada neste período.");

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

            return CreatedAtAction(nameof(GetLocacao), new { id = locacao.Id }, new LocacaoResponseDTO
            {
                Id = locacao.Id,
                EntregadorId = locacao.EntregadorId,
                MotoId = locacao.MotoId,
                DataInicio = locacao.DataInicio,
                DataTermino = locacao.DataTermino,
                DataPrevistaTermino = locacao.DataPrevistaTermino,
                ValorTotal = locacao.CalcularValorTotal()
            });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<LocacaoResponseDTO>> GetLocacao(Guid id)
        {
            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao == null) return NotFound();

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

        [HttpPost("simular-devolucao")]
        public async Task<ActionResult<SimularDevolucaoResponse>> SimularDevolucao([FromBody] SimularDevolucaoRequest request)
        {
            var locacao = await _context.Locacoes.FindAsync(request.LocacaoId);
            if (locacao == null)
                return NotFound("Locação não encontrada.");

            decimal valor = locacao.CalcularValorTotalParaSimulacao(request.NovaDataDevolucao);

            var response = new SimularDevolucaoResponse
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

            return Ok(response);
        }

        [HttpGet("consultar-por-entregador")]
        public async Task<IActionResult> ConsultarPorEntregador([FromQuery] string? cnpj, [FromQuery] string? numeroCnh)
        {
            if (string.IsNullOrEmpty(cnpj) && string.IsNullOrEmpty(numeroCnh))
                return BadRequest("Informe o CNPJ ou o número da CNH.");

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
            {
                query = query.Where(x => x.Cnpj == cnpj);
            }
            else if (!string.IsNullOrEmpty(numeroCnh))
            {
                query = query.Where(x => x.NumeroCNH == numeroCnh);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

    }

}
