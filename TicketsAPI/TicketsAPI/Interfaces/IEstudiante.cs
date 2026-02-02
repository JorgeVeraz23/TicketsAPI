using TicketsAPI.DTO;
using TicketsAPI.Entities;

namespace TicketsAPI.Interfaces
{
    public interface IEstudiante
    {

        // Crear un nuevo estudiante
        Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteCreateDto dto, CancellationToken ct);

            // Obtener un estudiante por su ID
            Task<EstudianteResponseDto> ObtenerEstudiantePorIdAsync(long id);

            // Obtener todos los estudiantes
            Task<List<EstudianteResponseDto>> ObtenerTodosEstudiantesAsync();

            // Actualizar los detalles de un estudiante
            Task<bool> ActualizarEstudianteAsync(long id, EstudianteCreateDto estudianteDto);

            // Eliminar un estudiante por su ID
            Task<bool> EliminarEstudianteAsync(long id);
            Task<List<EstudianteSearchDto>> SearchAsync(string query, int take = 10);
            Task<List<KeyValueDTO>> SelectorEstudiante();
            Task<List<KeyValueDTO>> SelectorEstudianteDocs();



    }
}
