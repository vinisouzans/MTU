using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTU.Model
{    
    public enum PlanoLocacao
    {
        Dias7 = 30,    // R$30/dia
        Dias15 = 28,   // R$28/dia
        Dias30 = 22,   // R$22/dia
        Dias45 = 20,   // R$20/dia
        Dias50 = 18    // R$18/dia
    }

    public class Locacao
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataTermino { get; set; }

        [Required]
        public DateTime DataPrevistaTermino { get; set; }

        [Required]
        public PlanoLocacao Plano { get; set; }

        [Required]
        public Guid EntregadorId { get; set; }

        [Required]
        public Guid MotoId { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        
        public void DefinirDataInicio()
        {
            DataInicio = DataCadastro.AddDays(1);
        }
        
        public decimal CalcularValorTotal()
        {
            decimal valorDiaria = (decimal)Plano;
            int totalDias = (int)(DataTermino.Date - DataInicio.Date).TotalDays + 1;
            int diasPrevistos = (int)(DataPrevistaTermino.Date - DataInicio.Date).TotalDays + 1;

            decimal valorTotal = valorDiaria * totalDias;

            if (DataTermino < DataPrevistaTermino)
            {
                // Devolução antecipada
                int diasNaoEfetivados = diasPrevistos - totalDias;
                decimal percentualMulta = Plano switch
                {
                    PlanoLocacao.Dias7 => 0.20m,
                    PlanoLocacao.Dias15 => 0.40m,
                    _ => 0
                };
                valorTotal += valorDiaria * diasNaoEfetivados * percentualMulta;
            }
            else if (DataTermino > DataPrevistaTermino)
            {
                // Devolução atrasada
                int diasExtras = (int)(DataTermino.Date - DataPrevistaTermino.Date).TotalDays;
                valorTotal += 50m * diasExtras; // R$50 por diária extra
            }

            return valorTotal;
        }
        
        public static bool EntregadorPodeAlugar(string tipoCNH)
        {
            return tipoCNH == "A" || tipoCNH == "A+B";
        }
    }

}
