using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Moto
{
    public class MotoUpdatePlacaDTO
    {
        [Required]
        [MaxLength(9)]
        public string NovaPlaca { get; set; }
    }
}
