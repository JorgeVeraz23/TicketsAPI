namespace TicketsAPI.DTO
{
    public class MatriculaPdfDto
    {
        public string Institucion { get; set; } = "";
        public string Periodo { get; set; } = "";
        public string CodigoMatricula { get; set; } = "";

        public string Estudiante { get; set; } = "";
        public string DocumentoEstudiante { get; set; } = "";
        public string Representante { get; set; } = "";
        public string DocumentoRepresentante { get; set; } = "";

        public string Grado { get; set; } = "";
        public string Paralelo { get; set; } = "";
        public DateTime Fecha { get; set; }

        public List<string> Materias { get; set; } = new();
    }

}
