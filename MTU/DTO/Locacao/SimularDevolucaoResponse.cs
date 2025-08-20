namespace MTU.DTO.Locacao
{
    public class SimularDevolucaoResponse
    {
        public Guid LocacaoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataPrevistaTermino { get; set; }
        public DateTime DataInformada { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }
}
