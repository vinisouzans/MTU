using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Entregador
{
    public class EntregadorCreateDTO
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$", ErrorMessage = "O CNPJ deve ter o formato 00.000.000/0000-00.")]
        public string Cnpj { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O número da CNH deve ter exatamente 11 dígitos.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O número da CNH deve conter apenas números.")]
        public string NumeroCNH { get; set; }

        [Required]
        [MaxLength(3)]
        public string TipoCNH { get; set; }
    }
}
