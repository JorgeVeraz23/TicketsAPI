namespace TicketsAPI.DTO
{
    public class AnioLectivoDTO
    {
        public long Id { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }

}
