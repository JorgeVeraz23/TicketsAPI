using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class TipoDocumentoDTO
    {
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string Codigo { get; set; } = default!;

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = default!;

        public bool EsObligatorio { get; set; } = true;

        public int Orden { get; set; } = 0;

        public bool Vigente { get; set; } = true;
    }
}
