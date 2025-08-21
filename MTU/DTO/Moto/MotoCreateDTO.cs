using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Moto
{
    public class MotoCreateDTO
    {
        [Required]
        public int Ano { get; set; }

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória.")]
        [RegularExpression(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$",
            ErrorMessage = "Placa inválida. Exemplo de placa válida (ex: BRA1A23).")]
        public string Placa { get; set; } = string.Empty;
    }
}
