using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IMateria
    {
        Task<MateriaResponseDto> CrearMateriaAsync(MateriaDTO materiaDto);
        Task<MateriaResponseDto> ObtenerMateriaPorIdAsync(int id);
        Task<List<MateriaResponseDto>> ObtenerMateriasPorGradoAsync(int gradoId);
        Task<bool> ActualizarMateriaAsync(int id, MateriaDTO materiaDto);
        Task<bool> EliminarMateriaAsync(int id);


    }
}
