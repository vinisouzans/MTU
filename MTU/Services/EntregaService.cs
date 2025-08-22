using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Entrega;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Services
{
    public class EntregaService : IEntregaService
    {
        private readonly AppDbContext _context;

        public EntregaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EntregaResponseDTO> CriarEntregaAsync(EntregaCreateDTO dto)
        {                       
            var pedido = await _context.Pedidos.FindAsync(dto.PedidoId);
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            if (pedido.Status != "ProntoParaEntrega")
                throw new InvalidOperationException($"Pedido não está pronto para entrega. Status atual: {pedido.Status}");

            var entregador = await _context.Entregadores.FindAsync(dto.EntregadorId);
            if (entregador == null)
                throw new ArgumentException("Entregador não encontrado");

            //VALIDAÇÃO DA LOCAÇÃO ATIVA
            if (!await EntregadorTemLocacaoAtivaAsync(dto.EntregadorId))
                throw new InvalidOperationException("Entregador não possui uma locação ativa de moto");

            var entrega = new Entrega
            {
                Id = Guid.NewGuid(),
                PedidoId = dto.PedidoId,
                EntregadorId = dto.EntregadorId,
                EnderecoRetirada = dto.EnderecoRetirada,
                EnderecoDestino = dto.EnderecoDestino,
                DataCriacao = DateTime.UtcNow,
                Status = "Disponivel"
            };
            
            _context.Entregas.Add(entrega);
            pedido.Status = "EmEntrega";
            await _context.SaveChangesAsync();

            return MapToResponse(entrega);
        }

        public async Task<IEnumerable<EntregaResponseDTO>> ListarEntregasAsync()
        {
            var entregas = await _context.Entregas
                .Include(e => e.Pedido)
                .Include(e => e.Entregador)
                .AsNoTracking()
                .ToListAsync();

            return entregas.Select(MapToResponse);
        }

        public async Task<EntregaResponseDTO> ObterEntregaAsync(Guid id)
        {
            var entrega = await _context.Entregas
                .Include(e => e.Pedido)
                .Include(e => e.Entregador)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entrega == null)
                throw new KeyNotFoundException("Entrega não encontrada");

            return MapToResponse(entrega);
        }

        public async Task AtualizarStatusAsync(Guid id, EntregaUpdateStatusDTO dto)
        {
            var entrega = await _context.Entregas
                .Include(e => e.Pedido)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entrega == null)
                throw new KeyNotFoundException("Entrega não encontrada");

            // Validar status válidos
            var statusValidos = new[] { "Disponivel", "EmAndamento", "Concluida", "Cancelada" };
            if (!statusValidos.Contains(dto.Status))
                throw new ArgumentException("Status inválido");

            // Se mudar para "EmAndamento" verificar locação
            if (dto.Status == "EmAndamento" && !await EntregadorTemLocacaoAtivaAsync(entrega.EntregadorId))
                throw new InvalidOperationException("Entregador não possui locação ativa para iniciar a entrega");

            entrega.Status = dto.Status;

            // Lógica para cada status
            switch (dto.Status)
            {
                case "EmAndamento":
                    entrega.DataInicio = DateTime.UtcNow;
                    break;

                case "Concluida":
                    entrega.DataConclusao = DateTime.UtcNow;

                    if (entrega.Pedido != null)
                    {
                        entrega.Pedido.Status = "Entregue";
                        entrega.Pedido.DataConclusao = DateTime.UtcNow;
                    }
                    else
                    {
                        throw new InvalidOperationException("Pedido associado à entrega não foi encontrado");
                    }
                    break;

                case "Cancelada":
                    entrega.DataConclusao = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoverEntregaAsync(Guid id)
        {
            var entrega = await _context.Entregas.FindAsync(id);
            if (entrega == null)
                throw new KeyNotFoundException("Entrega não encontrada");

            _context.Entregas.Remove(entrega);
            await _context.SaveChangesAsync();
        }

        private static EntregaResponseDTO MapToResponse(Entrega entrega)
        {
            return new EntregaResponseDTO
            {
                Id = entrega.Id,
                PedidoId = entrega.PedidoId,
                EntregadorId = entrega.EntregadorId,
                NomeEntregador = entrega.Entregador?.Nome,
                EnderecoRetirada = entrega.EnderecoRetirada,
                EnderecoDestino = entrega.EnderecoDestino,
                DataCriacao = entrega.DataCriacao,
                DataInicio = entrega.DataInicio,
                DataConclusao = entrega.DataConclusao,
                Status = entrega.Status
            };
        }

        public async Task<bool> EntregadorTemLocacaoAtivaAsync(Guid entregadorId)
        {
            var dataAtual = DateTime.UtcNow;

            var locacaoAtiva = await _context.Locacoes
                .Where(l => l.EntregadorId == entregadorId &&
                           l.DataInicio <= dataAtual &&
                           l.DataTermino >= dataAtual)                      
                .FirstOrDefaultAsync();

            return locacaoAtiva != null;
        }
    }
}
