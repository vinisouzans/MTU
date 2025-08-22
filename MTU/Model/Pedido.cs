namespace MTU.Model
{
    public class Pedido
    {
        public Guid Id { get; set; }
  
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public List<ItemPedido> Itens { get; set; } = new();

        public decimal ValorTotal { get; set; }
        public decimal? TaxaEntrega { get; set; }

        public string EnderecoRetirada { get; set; } = null!;
        public string EnderecoEntrega { get; set; } = null!;
        
        public string Status { get; set; } = "Criado";
        // Criado, EmPreparacao, ProntoParaEntrega, Entregue, Cancelado

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataConclusao { get; set; }
    }

}
