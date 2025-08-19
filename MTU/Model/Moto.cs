using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTU.Model
{    
    public class Moto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int Ano { get; set; }

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; }

        [Required]
        [MaxLength(10)]
        public string Placa { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public MotoCadastradaEvento GerarEvento()
        {
            return new MotoCadastradaEvento
            {
                MotoId = this.Id,
                Ano = this.Ano,
                Modelo = this.Modelo,
                Placa = this.Placa,
                DataEvento = DateTime.UtcNow
            };
        }

        public bool PrecisaConsumidorEspecial()
        {
            return this.Ano == 2024;
        }
    }

    public class MotoCadastradaEvento
    {
        public Guid MotoId { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public DateTime DataEvento { get; set; }
    }

}
