namespace MTU.Model
{
    public class Entrega
    {
        public Guid Id { get; set; }
       
        public Guid PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;
        public Guid EntregadorId { get; set; }

        // Localização
        public string EnderecoRetirada { get; set; } = null!;
        public string EnderecoDestino { get; set; } = null!;
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        
        public string Status { get; set; } = "Disponivel";
        // Disponivel, EmAndamento, Concluida, Cancelada
        
        public Entregador Entregador { get; set; } = null!;
    }
}
