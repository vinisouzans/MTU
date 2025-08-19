namespace MTU.DTO.Locacao
{
    public class LocacaoResponseDTO
    {
        public Guid Id { get; set; }
        public Guid EntregadorId { get; set; }
        public Guid MotoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevistaTermino { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
