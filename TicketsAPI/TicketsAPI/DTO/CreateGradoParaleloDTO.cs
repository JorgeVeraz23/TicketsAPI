namespace TicketsAPI.DTO
{
    public class CreateGradoParaleloDto
    {
        public long GradoId { get; set; }

        public long ParaleloId { get; set; }
        public long AnioLectivoId { get; set; }
        public int Cupos { get; set; }
    }

}
