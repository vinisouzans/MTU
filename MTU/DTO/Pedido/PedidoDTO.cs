using MTU.DTO.ItemPedido;

namespace MTU.DTO.Pedido
{
    public class PedidoDTO
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string NomeCliente { get; set; } = null!;
        public List<ItemPedidoDTO> Itens { get; set; } = new();
        public decimal ValorTotal { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public string EnderecoRetirada { get; set; } = null!;
        public string EnderecoEntrega { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
    }
}
