using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.ItemPedido
{
    public class ItemPedidoCreateDTO
    {
        [Required]
        public string NomeProduto { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecoUnitario { get; set; }
    }
}