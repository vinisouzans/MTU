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
        private readonly IWebHostEnvironment _env;

        public EntregadorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                CaminhoImagemCNH = GerarUrlAbsoluta(e.CaminhoImagemCNH)
            };
        }

        [HttpGet]
        public async Task<ActionResult<List<EntregadorResponseDTO>>> GetEntregadores()
        {
            var entregadores = await _context.Entregadores.ToListAsync();

            return entregadores.Select(e => new EntregadorResponseDTO
            {
                Id = e.Id,
                Nome = e.Nome,
                Cnpj = e.Cnpj,
                NumeroCNH = e.NumeroCNH,
                TipoCNH = e.TipoCNH,
                CaminhoImagemCNH = GerarUrlAbsoluta(e.CaminhoImagemCNH)
            }).ToList();
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
                CaminhoImagemCNH = GerarUrlAbsoluta(entregador.CaminhoImagemCNH)
            };
        }

        [HttpPost("{id}/upload-cnh")]
        public async Task<IActionResult> UploadCNH(Guid id, IFormFile arquivo)
        {
            var entregador = await _context.Entregadores.FindAsync(id);
            if (entregador == null)
                return NotFound("Entregador não encontrado");

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido");

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (extensao != ".png" && extensao != ".bmp")
                return BadRequest("Formato inválido. Apenas PNG ou BMP são aceitos.");

            // Criar diretório caso não exista
            var pastaUploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "cnhs");
            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            // Nome do arquivo: {id}{extensão}
            var nomeArquivo = $"{id}{extensao}";
            var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

            // Salvar no disco
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // Atualizar o caminho no banco
            entregador.CaminhoImagemCNH = $"/uploads/cnhs/{nomeArquivo}";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Upload realizado com sucesso!",
                caminho = GerarUrlAbsoluta(entregador.CaminhoImagemCNH)
            });
        }

        private string GerarUrlAbsoluta(string caminhoRelativo)
        {
            if (string.IsNullOrEmpty(caminhoRelativo))
                return null;

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{caminhoRelativo}";
        }


    }

}
