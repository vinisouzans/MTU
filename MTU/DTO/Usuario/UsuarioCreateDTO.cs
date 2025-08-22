using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Usuario
{
    public class UsuarioCreateDTO
    {
        public string Nome { get; set; }

        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool EhAdmin { get; set; } = false;
    }
}
