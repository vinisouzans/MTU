using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Moto;
using MTU.Model;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MotoResponseDTO>> CriarMoto(MotoCreateDTO dto)
        {
            var moto = new Moto
            {
                Ano = dto.Ano,
                Modelo = dto.Modelo,
                Placa = dto.Placa
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            
            var evento = moto.GerarEvento();
            
            if (moto.PrecisaConsumidorEspecial())
            {
                // Armazenar ou processar moto de 2024
            }

            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            });
        }

        [HttpPut("{id}/placa")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MotoResponseDTO>> AtualizarPlaca(Guid id, [FromBody] MotoUpdatePlacaDTO dto)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound("Moto não encontrada");
            
            var placaExistente = await _context.Motos.AnyAsync(m => m.Placa == dto.NovaPlaca);
            if (placaExistente) return BadRequest("Essa placa já está cadastrada");

            moto.Placa = dto.NovaPlaca;
            await _context.SaveChangesAsync();

            return Ok(new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverMoto(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound("Moto não encontrada");

            // Verifica se há locações vinculadas
            var possuiLocacoes = await _context.Locacoes.AnyAsync(l => l.MotoId == id);
            if (possuiLocacoes)
                return BadRequest("Não é possível remover a moto pois existem locações registradas");

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return Ok("Moto removida com sucesso");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponseDTO>> GetMoto(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) return NotFound();

            return new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };
        }

        [HttpGet]
        public async Task<ActionResult<List<MotoResponseDTO>>> GetMotos([FromQuery] string? placa)
        {
            var query = _context.Motos.AsQueryable();
            if (!string.IsNullOrEmpty(placa))
                query = query.Where(m => m.Placa == placa);

            var motos = await query.ToListAsync();

            return motos.Select(m => new MotoResponseDTO
            {
                Id = m.Id,
                Ano = m.Ano,
                Modelo = m.Modelo,
                Placa = m.Placa
            }).ToList();
        }
    }

}
