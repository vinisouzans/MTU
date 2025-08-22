using MTU.Data;
using MTU.DTO.Pedido;
using MTU.DTO.ItemPedido;
using MTU.Model;
using MTU.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MTU.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;

        public PedidoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PedidoDTO> CriarPedidoAsync(PedidoCreateDTO dto)
        {
            // Validar se cliente existe
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado");

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = dto.ClienteId,
                TaxaEntrega = dto.TaxaEntrega,
                EnderecoRetirada = dto.EnderecoRetirada,
                EnderecoEntrega = dto.EnderecoEntrega,
                Status = "Criado",
                DataCriacao = DateTime.UtcNow
            };

            // Adicionar itens e calcular valor total
            decimal valorTotal = 0;
            foreach (var itemDto in dto.Itens)
            {
                var item = new ItemPedido
                {
                    Id = Guid.NewGuid(),
                    PedidoId = pedido.Id,
                    Produto = itemDto.NomeProduto,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = itemDto.PrecoUnitario
                };

                valorTotal += item.Subtotal;
                pedido.Itens.Add(item);
            }

            pedido.ValorTotal = valorTotal + (dto.TaxaEntrega ?? 0);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(pedido.Id);
        }

        public async Task<PedidoDTO> ObterPorIdAsync(Guid id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            return MapToDTO(pedido);
        }

        public async Task<List<PedidoDTO>> ObterTodosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(MapToDTO).ToList();
        }

        public async Task<PedidoDTO> AtualizarStatusAsync(Guid id, string status)
        {
            var statusValidos = new[] { "Criado", "EmPreparacao", "ProntoParaEntrega", "Entregue", "Cancelado" };
            if (!statusValidos.Contains(status))
                throw new ArgumentException("Status inválido");

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            pedido.Status = status;

            if (status == "Entregue" || status == "Cancelado")
                pedido.DataConclusao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(id);
        }

        public async Task<bool> CancelarPedidoAsync(Guid id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            if (pedido.Status == "Entregue")
                throw new InvalidOperationException("Não é possível cancelar um pedido já entregue");

            pedido.Status = "Cancelado";
            pedido.DataConclusao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PedidoDTO>> ObterPorClienteAsync(Guid clienteId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(MapToDTO).ToList();
        }

        public async Task<List<PedidoDTO>> ObterPedidosProntosParaEntregaAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .Where(p => p.Status == "ProntoParaEntrega")
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return pedidos.Select(MapToDTO).ToList();
        }

        private PedidoDTO MapToDTO(Pedido pedido)
        {
            return new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = pedido.Cliente?.Nome ?? "Cliente não encontrado",
                Itens = pedido.Itens.Select(i => new ItemPedidoDTO
                {
                    Id = i.Id,
                    NomeProduto = i.Produto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList(),
                ValorTotal = pedido.ValorTotal,
                TaxaEntrega = pedido.TaxaEntrega,
                EnderecoRetirada = pedido.EnderecoRetirada,
                EnderecoEntrega = pedido.EnderecoEntrega,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                DataConclusao = pedido.DataConclusao
            };
        }        
    }
}