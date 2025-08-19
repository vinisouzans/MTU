namespace MTU.DTO.Usuario
{
    public class UsuarioCreateDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool EhAdmin { get; set; } = false;
    }
}
