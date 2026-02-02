namespace TicketsAPI.DTO
{
    public class DashboardResumenDto
    {
        public DashboardKpisDto Kpis { get; set; } = new();
        public List<DashboardMatriculasPorGradoDto> MatriculasPorGrado { get; set; } = new();
        public List<DashboardDocumentoRecienteDto> UltimosDocumentos { get; set; } = new();
    }

    public class DashboardKpisDto
    {
        public string Periodo { get; set; } = "";
        public long EstudiantesActivos { get; set; }
        public long MatriculasPendientes { get; set; }
        public long DocumentosPorValidar { get; set; }
    }

    public class DashboardMatriculasPorGradoDto
    {
        public long GradoId { get; set; }
        public string GradoNombre { get; set; } = "";
        public int Total { get; set; }
    }

    public class DashboardDocumentoRecienteDto
    {
        public long DocumentoId { get; set; }
        public string DocumentoNombre { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime FechaCreacion { get; set; }

        public long EstudianteId { get; set; }
        public string EstudianteNombreCompleto { get; set; } = "";

        public long TipoDocumentoId { get; set; }
        public string TipoDocumentoNombre { get; set; } = "";
    }

}
