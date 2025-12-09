using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface ICurso
    {
        public Task<List<CursoDTO>> GetAllCursos();
        public Task<CursoDTO> GetCursoById(long id); 
        public Task<bool> CrearCurso(CursoDTO cursoDTO);
        public Task<bool> EditarCurso(CursoDTO cursotDTO);
        public Task<bool> EliminarCurso(long idCurso);
    }
}
