using MTU.DTO.ItemPedido;
using MTU.DTO.Pedido;
using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Pedido
{
    public class PedidoCreateDTO
    {
        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        public List<ItemPedidoCreateDTO> Itens { get; set; } = new();

        public decimal? TaxaEntrega { get; set; }

        [Required]
        public string EnderecoRetirada { get; set; } = null!;

        [Required]
        public string EnderecoEntrega { get; set; } = null!;
    }
}
