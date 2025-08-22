namespace MTU.DTO.Entrega
{
    public class EntregaCreateDTO
    {
        public Guid PedidoId { get; set; }
        public Guid EntregadorId { get; set; }

        public string EnderecoRetirada { get; set; } = null!;
        public string EnderecoDestino { get; set; } = null!;
    }
}
