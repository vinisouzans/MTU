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

        [Required]
        [MaxLength(10)]
        public string Placa { get; set; }
    }
}
