using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IAnioLectiivo
    {
        public Task<AnioLectivoDTO> GetAnioLectivoById(long id);
        public Task<List<AnioLectivoDTO>> GetAllAnioLectivo();
        public Task<List<KeyValueDTO>> SelectorAnioLectivo();
        public Task<bool> CrearAnioLectivo(AnioLectivoDTO anioLectivoDTO);
        public Task<bool> EditarAnioLectivo(AnioLectivoDTO anioLectivoDTO);
        public Task<bool> EliminarAnioLectivo(long id);

    }
}
