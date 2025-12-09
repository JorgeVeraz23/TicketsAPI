namespace TicketsAPI.DTO
{
    public class RepresentanteDTO
    {
        public long IdRepresentante { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Identificacion { get; set; }
        public string Celular { get; set; }

        public DateTime FechaNacimiento { get; set; }
    }
}
