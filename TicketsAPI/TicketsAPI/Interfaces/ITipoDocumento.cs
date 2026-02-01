using TicketsAPI.DTO;
using TicketsAPI.Entities;

namespace TicketsAPI.Interfaces
{
    public interface ITipoDocumento
    {
        Task<List<KeyValueDTO>> ListAsync();
    }
}
