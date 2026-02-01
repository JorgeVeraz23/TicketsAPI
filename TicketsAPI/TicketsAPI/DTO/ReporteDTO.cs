namespace TicketsAPI.DTO
{
    public class ReporteMatriculasFiltroDto
    {
        public int? AnioLectivoId { get; set; }
        public int? GradoParaleloId { get; set; }

        // filtros de búsqueda
        public string? CedulaEstudiante { get; set; }
        public string? Texto { get; set; } // nombres / apellidos

        // rango por fecha de matrícula
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ReporteMatriculaItemDto
    {
        public int MatriculaId { get; set; }
        public string CodigoMatricula { get; set; } = "";
        public DateTime Fecha { get; set; }

        public int EstudianteId { get; set; }
        public string Estudiante { get; set; } = "";
        public string? CedulaEstudiante { get; set; }

        public long? RepresentanteId { get; set; }
        public string? Representante { get; set; }
        public string? CedulaRepresentante { get; set; }

        public int AnioLectivoId { get; set; }
        public string AnioLectivo { get; set; } = "";

        public int GradoParaleloId { get; set; }
        public string GradoParalelo { get; set; } = "";

        public string Estado { get; set; } = "";
    }

    public class PagedResultDto<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
