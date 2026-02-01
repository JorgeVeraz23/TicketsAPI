using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IReportesServices
    {
        Task<PagedResultDto<ReporteMatriculaItemDto>> ObtenerMatriculasAsync(ReporteMatriculasFiltroDto filtro);

    }
}
