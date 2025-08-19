using Microsoft.AspNetCore.Mvc;
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

            var locacao = new Locacao
            {
                EntregadorId = dto.EntregadorId,
                MotoId = dto.MotoId,
                Plano = dto.Plano,
                DataPrevistaTermino = dto.DataTermino,
                DataTermino = dto.DataTermino
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
    }

}
