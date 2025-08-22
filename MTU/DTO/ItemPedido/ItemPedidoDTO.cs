namespace MTU.DTO.ItemPedido
{
    public class ItemPedidoDTO
    {
        public Guid Id { get; set; }
        public string NomeProduto { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
