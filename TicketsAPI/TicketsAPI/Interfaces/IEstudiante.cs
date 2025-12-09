using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IEstudiante
    {
        public Task<EstudianteDTO> GetEstudianteById(long id);
        public Task<List<EstudianteDTO>> GetAllEstudiante();
        public Task<bool> EditarEstudiante(EstudianteDTO estudianteDTO);
        public Task<bool> EliminarEstudiante(long idEstudiante);
        public Task<bool> CrearEstudiante(EstudianteDTO estudianteDTO);
    }
}
