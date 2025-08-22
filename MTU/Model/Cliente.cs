namespace MTU.Model
{
    public class Cliente
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Endereco { get; set; } = null!;
        public DateTime DataCadastro { get; set; }


        public List<Pedido> Pedidos { get; set; } = new();
    }

}
