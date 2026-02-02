using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IDashboardService
    {
        
            Task<DashboardResumenDto> GetResumenAsync(long anioLectivoId, CancellationToken ct);
       


    }
}
