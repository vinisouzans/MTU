using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTU.Model
{    
    public class Entregador
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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
       
        [MaxLength(250)]
        public string? CaminhoImagemCNH { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public static bool TipoCNHValido(string tipoCNH)
        {
            return tipoCNH == "A" || tipoCNH == "B" || tipoCNH == "AB";
        }
    }

}
