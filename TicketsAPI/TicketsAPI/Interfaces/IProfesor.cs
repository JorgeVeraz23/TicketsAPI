using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IProfesor
    {
        public Task<ProfesorDTO> GetProfesorById(long idProfesor);
        public Task<List<ProfesorDTO>> GetAllProfesor();

        public Task<bool> CrearProfesor(ProfesorDTO profesorDTO);
        public Task<bool> EditarProfesor(ProfesorDTO profesorDTO);
        public Task<bool> EliminarProfesor(long idProfesor);
    }
}
