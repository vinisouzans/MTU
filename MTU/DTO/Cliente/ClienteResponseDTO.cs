namespace MTU.DTO.Cliente
{
    public class ClienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Endereco { get; set; } = null!;
    }
}
