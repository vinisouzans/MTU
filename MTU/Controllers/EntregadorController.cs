using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Entregador;
using MTU.Model;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/entregadores")]
    public class EntregadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EntregadorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<EntregadorResponseDTO>> CriarEntregador(EntregadorCreateDTO dto)
        {
            if (!Entregador.TipoCNHValido(dto.TipoCNH))
                return BadRequest("Tipo CNH inválido");

            if (_context.Entregadores.Any(e => e.Cnpj == dto.Cnpj))
                return BadRequest("CNPJ já cadastrado");

            if (_context.Entregadores.Any(e => e.NumeroCNH == dto.NumeroCNH))
                return BadRequest("CNH já cadastrada");

            var entregador = new Entregador
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                DataNascimento = dto.DataNascimento,
                NumeroCNH = dto.NumeroCNH,
                TipoCNH = dto.TipoCNH
            };

            _context.Entregadores.Add(entregador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntregador), new { id = entregador.Id }, new EntregadorResponseDTO
            {
                Id = entregador.Id,
                Nome = entregador.Nome,
                Cnpj = entregador.Cnpj,
                NumeroCNH = entregador.NumeroCNH,
                TipoCNH = entregador.TipoCNH
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntregadorResponseDTO>> GetEntregador(Guid id)
        {
            var e = await _context.Entregadores.FindAsync(id);
            if (e == null) return NotFound();

            return new EntregadorResponseDTO
            {
                Id = e.Id,
                Nome = e.Nome,
                Cnpj = e.Cnpj,
                NumeroCNH = e.NumeroCNH,
                TipoCNH = e.TipoCNH,
                CaminhoImagemCNH = e.CaminhoImagemCNH
            };
        }

        [HttpGet]
        public async Task<ActionResult<List<EntregadorResponseDTO>>> GetEntregadores()
        {
            return await _context.Entregadores
                .Select(e => new EntregadorResponseDTO
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Cnpj = e.Cnpj,
                    NumeroCNH = e.NumeroCNH,
                    TipoCNH = e.TipoCNH,
                    CaminhoImagemCNH = e.CaminhoImagemCNH
                }).ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EntregadorResponseDTO>> AtualizarEntregador(Guid id, EntregadorUpdateDTO dto)
        {
            var entregador = await _context.Entregadores.FindAsync(id);
            if (entregador == null) return NotFound();

            if (!Entregador.TipoCNHValido(dto.TipoCNH))
                return BadRequest("Tipo CNH inválido");
            
            if (_context.Entregadores.Any(e => e.Cnpj == dto.Cnpj && e.Id != id))
                return BadRequest("CNPJ já cadastrado");
            
            if (_context.Entregadores.Any(e => e.NumeroCNH == dto.NumeroCNH && e.Id != id))
                return BadRequest("CNH já cadastrada");
            
            entregador.Nome = dto.Nome;
            entregador.Cnpj = dto.Cnpj;
            entregador.NumeroCNH = dto.NumeroCNH;
            entregador.TipoCNH = dto.TipoCNH;

            await _context.SaveChangesAsync();

            return new EntregadorResponseDTO
            {
                Id = entregador.Id,
                Nome = entregador.Nome,
                Cnpj = entregador.Cnpj,
                NumeroCNH = entregador.NumeroCNH,
                TipoCNH = entregador.TipoCNH,
                CaminhoImagemCNH = entregador.CaminhoImagemCNH
            };
        }


    }

}
