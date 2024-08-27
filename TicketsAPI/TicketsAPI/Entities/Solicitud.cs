using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.DTO;

namespace TicketsAPI.Entities
{
    public class Solicitud
    {
        [Key]
        public long IdSolicitud { get; set; }
        public EnumTipoSolicitud tipoSolicitud { get; set; }
        public string DescripcionSolicitud { get; set; }
        public string Justificativo { get; set; }
        public EnumEstadoSolicitud estadoSolicitud { get; set; }
        public string DetalleGestion { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaGestion { get; set; }
        [ForeignKey("Usuario")]
        public long IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
       

    }
}

