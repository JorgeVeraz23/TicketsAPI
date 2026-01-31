using System.Threading.Tasks;
using TicketsAPI.DTO;
using TicketsAPI.Entities;

namespace TicketsAPI.Interfaces
{
    public interface IRepresentante
    {
        Task<List<Representante>> ListAsync(string? q, CancellationToken ct);
        Task<Representante?> GetByIdAsync(long id, CancellationToken ct);
  
        Task<bool> ExistsByDocumentoAsync(string tipoDocumento, string numeroDocumento, long? excludeId, CancellationToken ct);

        Task AddAsync(Representante entity, CancellationToken ct);
        Task UpdateAsync(Representante entity, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }

}
