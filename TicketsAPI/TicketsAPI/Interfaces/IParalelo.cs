using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IParalelo
    {
        // Crear un nuevo paralelo
        Task<ParaleloResponseDto> CrearParaleloAsync(ParaleloDTO paraleloDto);

        // Obtener un paralelo por su ID
        Task<ParaleloResponseDto> ObtenerParaleloPorIdAsync(long id);
        Task<List<KeyValueDTO>> SelectorParalelo();


        // Obtener todos los paralelos
        Task<List<ParaleloResponseDto>> ObtenerParalelosAsync();

        // Actualizar un paralelo existente
        Task<bool> ActualizarParaleloAsync(long id, ParaleloDTO paraleloDto);

        // Eliminar un paralelo por su ID
        Task<bool> EliminarParaleloAsync(long id);
    }
}
