using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Cliente
{
    public class ClienteCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string Telefone { get; set; } = null!;

        [Required]
        public string Endereco { get; set; } = null!;
    }
}
