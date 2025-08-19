using MTU.Model;
using System.ComponentModel.DataAnnotations;

namespace MTU.DTO.Locacao
{
    public class LocacaoCreateDTO
    {
        [Required]
        public Guid EntregadorId { get; set; }

        [Required]
        public Guid MotoId { get; set; }

        [Required]
        public PlanoLocacao Plano { get; set; }

        [Required]
        public DateTime DataTermino { get; set; }
    }
}
