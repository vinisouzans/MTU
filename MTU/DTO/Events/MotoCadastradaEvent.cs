namespace MTU.DTO.Events
{
    public class MotoCadastradaEvent
    {
        public Guid Id { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
    }
}
