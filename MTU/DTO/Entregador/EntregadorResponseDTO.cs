namespace MTU.DTO.Entregador
{
    public class EntregadorResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string NumeroCNH { get; set; }
        public string TipoCNH { get; set; }
        public string CaminhoImagemCNH { get; set; }
    }
}
