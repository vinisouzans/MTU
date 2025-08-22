namespace MTU.DTO.Entrega
{
    public class EntregaResponseDTO
    {
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public Guid EntregadorId { get; set; }
        public string NomeEntregador { get; set; }

        public string EnderecoRetirada { get; set; } = null!;
        public string EnderecoDestino { get; set; } = null!;

        public DateTime DataCriacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }

        public string Status { get; set; } = null!;
    }
}
