using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Moto;
using MTU.Events;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Services
{
    public class MotoService : IMotoService
    {
        private readonly AppDbContext _context;
        private readonly IMotoPublisher _publisher;

        public MotoService(AppDbContext context, IMotoPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<MotoResponseDTO> CriarMotoAsync(MotoCreateDTO dto)
        {
            var existe = await _context.Motos.AnyAsync(m => m.Placa == dto.Placa);
            if (existe)
                throw new InvalidOperationException("Já existe uma moto cadastrada com essa placa.");

            var moto = new Moto
            {
                Ano = dto.Ano,
                Modelo = dto.Modelo,
                Placa = dto.Placa
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            
            var evento = new MotoCadastradaEvent
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };

            _publisher.Publicar("motos_cadastradas", evento);

            return new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };
        }

        public async Task<MotoResponseDTO> AtualizarPlacaAsync(Guid id, MotoUpdatePlacaDTO dto)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) throw new KeyNotFoundException("Moto não encontrada");

            var placaExistente = await _context.Motos.AnyAsync(m => m.Placa == dto.NovaPlaca);
            if (placaExistente) throw new InvalidOperationException("Essa placa já está cadastrada");

            moto.Placa = dto.NovaPlaca;
            await _context.SaveChangesAsync();

            return new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };
        }

        public async Task RemoverMotoAsync(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                throw new KeyNotFoundException("Moto não encontrada");

            var possuiLocacoes = await _context.Locacoes.AnyAsync(l => l.MotoId == id);
            if (possuiLocacoes)
                throw new InvalidOperationException("Não é possível remover a moto pois existem locações registradas");

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
        }

        public async Task<MotoResponseDTO> ObterMotoAsync(Guid id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null) throw new KeyNotFoundException("Moto não encontrada");

            return new MotoResponseDTO
            {
                Id = moto.Id,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
            };
        }

        public async Task<List<MotoResponseDTO>> ObterMotosAsync(string? placa = null)
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
