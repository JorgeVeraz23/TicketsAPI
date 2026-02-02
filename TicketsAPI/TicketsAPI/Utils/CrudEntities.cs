using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Utils
{
    public class CrudEntities
    {
        [MaxLength(100)]
        public string? UsuarioCreacion { get; set; }
        [MaxLength(100)]
        public string? UsuarioModificacion { get; set; }
        [MaxLength(100)]
        public string? UsuarioEliminacion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaEliminacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActive { get; set; } = true;
    }

    //public enum EstadoEstudiante { Activo = 1, Inactivo = 2, Retirado = 3 }
    //public enum Genero { Masculino = 1, Femenino = 2, Otro = 3 }

    //public enum EstadoMatricula { Pendiente = 1, Confirmada = 2, Anulada = 3 }
    //public enum EstadoPago { Pendiente = 1, Pagado = 2, Anulado = 3 }

    //public enum EstadoDocumento { Pendiente = 1, Aprobado = 2, Rechazado = 3 }
}
