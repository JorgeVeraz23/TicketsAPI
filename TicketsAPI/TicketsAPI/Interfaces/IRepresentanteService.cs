using TicketsAPI.DTO;
using TicketsAPI.Entities;

namespace TicketsAPI.Interfaces
{
    public interface IRepresentanteService
    {
        Task<List<Representante>> ListAsync(string? q, CancellationToken ct);
        Task<Representante> GetAsync(long id, CancellationToken ct);
        Task<long> CreateAsync(RepresentanteCreateDto dto, string userName, CancellationToken ct);
        Task UpdateAsync(long id, RepresentanteUpdateDto dto, string userName, CancellationToken ct);
        Task DeleteAsync(long id, string userName, CancellationToken ct);
        Task<List<KeyValueDTO>> SelectorRepresentanteAsync();

    }
}
