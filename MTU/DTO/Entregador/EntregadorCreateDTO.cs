using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Entregador
{
    public class EntregadorCreateDTO
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cnpj { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        [MaxLength(20)]
        public string NumeroCNH { get; set; }

        [Required]
        [MaxLength(3)]
        public string TipoCNH { get; set; }
    }
}
