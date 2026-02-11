using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IProfesor
    {
        Task<ProfesorResponseDto> CrearProfesorAsync(ProfesorCreateDto dto);
        Task<ProfesorResponseDto?> ObtenerProfesorPorIdAsync(long id);
        Task<List<ProfesorResponseDto>> ObtenerTodosProfesoresAsync();
        Task<List<KeyValueDTO>> SelectorProfesor();
        Task<List<KeyValueDTO>> SelectorProfesorTutor();
        Task<bool> ActualizarProfesorAsync(long id, ProfesorCreateDto dto);
        Task<bool> EliminarProfesorAsync(long id);
        Task<List<ProfesorSearchDto>> SearchAsync(string query, int take = 10);
    }
}
