using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Entregador;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly AppDbContext _context;

        public EntregadorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EntregadorResponseDTO> CriarEntregadorAsync(EntregadorCreateDTO dto)
        {
            if (!Entregador.TipoCNHValido(dto.TipoCNH))
                throw new ArgumentException("Tipo CNH inválido");

            if (await _context.Entregadores.AnyAsync(e => e.Cnpj == dto.Cnpj))
                throw new ArgumentException("CNPJ já cadastrado");

            if (await _context.Entregadores.AnyAsync(e => e.NumeroCNH == dto.NumeroCNH))
                throw new ArgumentException("CNH já cadastrada");

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

            return new EntregadorResponseDTO
            {
                Id = entregador.Id,
                Nome = entregador.Nome,
                Cnpj = entregador.Cnpj,
                NumeroCNH = entregador.NumeroCNH,
                TipoCNH = entregador.TipoCNH
            };
        }

        public async Task<EntregadorResponseDTO> AtualizarEntregadorAsync(Guid id, EntregadorUpdateDTO dto)
        {
            var entregador = await _context.Entregadores.FindAsync(id);
            if (entregador == null)
                throw new KeyNotFoundException("Entregador não encontrado");

            if (!Entregador.TipoCNHValido(dto.TipoCNH))
                throw new ArgumentException("Tipo CNH inválido");

            if (await _context.Entregadores.AnyAsync(e => e.Cnpj == dto.Cnpj && e.Id != id))
                throw new ArgumentException("CNPJ já cadastrado");

            if (await _context.Entregadores.AnyAsync(e => e.NumeroCNH == dto.NumeroCNH && e.Id != id))
                throw new ArgumentException("CNH já cadastrada");

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

        public async Task<EntregadorResponseDTO> ObterEntregadorAsync(Guid id)
        {
            var e = await _context.Entregadores.FindAsync(id);
            if (e == null)
                throw new KeyNotFoundException("Entregador não encontrado");

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

        public async Task<List<EntregadorResponseDTO>> ObterTodosEntregadoresAsync()
        {
            var entregadores = await _context.Entregadores.ToListAsync();
            return entregadores.Select(e => new EntregadorResponseDTO
            {
                Id = e.Id,
                Nome = e.Nome,
                Cnpj = e.Cnpj,
                NumeroCNH = e.NumeroCNH,
                TipoCNH = e.TipoCNH,
                CaminhoImagemCNH = e.CaminhoImagemCNH
            }).ToList();
        }

        public async Task<string> UploadCNHAsync(Guid id, IFormFile arquivo, string webRootPath, HttpRequest request)
        {
            var entregador = await _context.Entregadores.FindAsync(id);
            if (entregador == null)
                throw new KeyNotFoundException("Entregador não encontrado");

            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (extensao != ".png" && extensao != ".bmp")
                throw new ArgumentException("Formato inválido. Apenas PNG ou BMP são aceitos.");

            var pastaUploads = Path.Combine(webRootPath ?? "wwwroot", "uploads", "cnhs");
            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var nomeArquivo = $"{id}{extensao}";
            var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            entregador.CaminhoImagemCNH = $"/uploads/cnhs/{nomeArquivo}";
            await _context.SaveChangesAsync();

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{entregador.CaminhoImagemCNH}";
        }
    }
}
