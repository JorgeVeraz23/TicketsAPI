namespace TicketsAPI.DTO
{
    public class SolicitudDTO
    {
        public long IdSolicitud { get; set; }
        public EnumTipoSolicitud tipoSolicitud { get; set; }
        public string DescripcionSolicitud { get; set; }
        public string Justificativo { get; set; }
        public EnumEstadoSolicitud estadoSolicitud { get; set; }
        public string DetalleGestion { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaGestion { get; set; }
        public long IdUsuario { get; set; }
    }
}
