namespace MTU.Model
{
    public class NotificacaoMoto
    {
        public Guid Id { get; set; }
        public Guid MotoId { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public DateTime DataRecebida { get; set; } = DateTime.Now;
    }
}
