using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Cliente
{
    public class ClienteUpdateDTO
    {
        [StringLength(100)]
        public string? Nome { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? Telefone { get; set; }

        public string? Endereco { get; set; }
    }
}
