using TicketsAPI.DTO;
using TicketsAPI.Entities;

namespace TicketsAPI.Interfaces
{
    public interface IEstudiante
    {
    
            // Crear un nuevo estudiante
            Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteDTO estudianteDto);

            // Obtener un estudiante por su ID
            Task<EstudianteResponseDto> ObtenerEstudiantePorIdAsync(long id);

            // Obtener todos los estudiantes
            Task<List<EstudianteResponseDto>> ObtenerTodosEstudiantesAsync();

            // Actualizar los detalles de un estudiante
            Task<bool> ActualizarEstudianteAsync(long id, EstudianteDTO estudianteDto);

            // Eliminar un estudiante por su ID
            Task<bool> EliminarEstudianteAsync(long id);
            Task<List<EstudianteSearchDto>> SearchAsync(string query, int take = 10);



    }
}
