using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface ICalificacion
    {
        Task<CalificacionResponseDto> CrearAsync(CalificacionCreateDto dto);
        Task<CalificacionResponseDto?> ObtenerPorIdAsync(long id);

        Task<List<CalificacionResponseDto>> ListarAsync(string? periodo, string? materia);

        Task<List<CalificacionResponseDto>> ListarPorEstudianteAsync(long estudianteId, string? periodo);
        Task<List<CalificacionResponseDto>> ListarPorProfesorAsync(long profesorId, string? periodo);

        Task<bool> ActualizarAsync(long id, CalificacionCreateDto dto);
        Task<bool> EliminarAsync(long id);
    }
}
