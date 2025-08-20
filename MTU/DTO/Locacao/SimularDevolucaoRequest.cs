namespace MTU.DTO.Locacao
{
    public class SimularDevolucaoRequest
    {
        public Guid LocacaoId { get; set; }
        public DateTime NovaDataDevolucao { get; set; }
    }
}
