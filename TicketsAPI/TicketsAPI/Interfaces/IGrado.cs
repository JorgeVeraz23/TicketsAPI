using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IGrado
    {
        public Task<List<KeyValueDTO>> SelectorGrado();
    }
}
