namespace MTU.Events
{
    public class MotoCadastradaEvent
    {
        public Guid Id { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public DateTime DataEvento { get; set; } = DateTime.UtcNow;
    }
}
