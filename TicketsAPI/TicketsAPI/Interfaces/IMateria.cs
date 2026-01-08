using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IMateria
    {
        Task<MateriaResponseDto> CrearMateriaAsync(MateriaDTO materiaDto);
        Task<List<KeyValueDTO>> SelectorMateria();
        Task<MateriaResponseDto> ObtenerMateriaPorIdAsync(long id);
        Task<List<MateriaResponseDto>> ObtenerMateriasPorGradoAsync(long gradoId);
        Task<List<MateriaResponseDto>> GetAllMaterias(long? idGrado);
        Task<bool> ActualizarMateriaAsync(long id, MateriaDTO materiaDto);
        Task<bool> EliminarMateriaAsync(long id);


    }
}
