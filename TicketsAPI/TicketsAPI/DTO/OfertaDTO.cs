namespace TicketsAPI.DTO
{
    public class OfertaDTO
    {
        public long gradoParaleloId { get; set; }
        public long paraleloId { get; set; }
        public string gradoNombre { get; set; }
        public string paraleloNombre { get; set; }
        public string anioLectivo   { get; set; }
        public int cupos { get; set; }
        public int ocupados { get; set; }
        public int disponibles { get; set; }

    }
}
