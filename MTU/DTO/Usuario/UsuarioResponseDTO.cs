namespace MTU.DTO.Usuario
{
    public class UsuarioResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool EhAdmin { get; set; }
    }
}
