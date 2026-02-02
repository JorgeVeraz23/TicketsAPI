using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{

    public enum TipoPeriodoEvaluativo
    {
        PARCIAL = 1,
        QUIMESTRE = 2
    }

    public class PeriodoEvaluativo : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("AnioLectivo")]
        public long AnioLectivoId { get; set; }
        public virtual AnioLectivo AnioLectivo { get; set; } = default!;

        public TipoPeriodoEvaluativo Tipo { get; set; }  // PARCIAL

        public int Numero { get; set; } // 1,2,3,4

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = default!; // "Parcial 1"

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
