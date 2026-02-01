namespace TicketsAPI.Interfaces
{
    public interface IMatriculaPdfService
    {
        Task<byte[]> GenerarPdfAsync(int matriculaId);
    }

}
