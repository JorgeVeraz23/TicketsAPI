namespace TicketsAPI.DTO
{
    public class SolicitudDTO
    {
        public long IdSolicitud { get; set; }
        public EnumTipoSolicitud tipoSolicitud { get; set; }
        public string DescripcionSolicitud { get; set; }
        public IFormFile Justificativo { get; set; } // Este debe ser IFormFile
        public string DetalleGestion { get; set; }
        public DateTime FechaIngreso { get; set; }

        public long IdUsuario { get; set; }
    }

    public class MostrarSolicitudDTO
    {
        public long IdSolicitud { get; set; }
        public EnumTipoSolicitud tipoSolicitud { get; set; }
        public string DescripcionSolicitud { get; set; }
        public string Justificativo { get; set; } // Este debe ser IFormFile

        public string DetalleGestion { get; set; }
        public DateTime FechaIngreso { get; set; }

        public long IdUsuario { get; set; }
    }

    public class MostrarJustificativoDTO
    {
        public string idSolicitud { get; set; }
        public string Justificativo { get; set; }
    }
}
