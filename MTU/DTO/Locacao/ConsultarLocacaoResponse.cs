namespace MTU.DTO.Locacao
{
    public class ConsultarLocacaoResponse
    {
        public Guid LocacaoId { get; set; }
        public Guid EntregadorId { get; set; }
        public string NomeEntregador { get; set; }
        public string Documento { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataPrevistaTermino { get; set; }
        public DateTime? DataTermino { get; set; }
        public decimal ValorEstimado { get; set; }
        public string Status { get; set; }
    }
}
