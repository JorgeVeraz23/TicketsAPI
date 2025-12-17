using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IMateria
    {
        public Task<List<MateriaDTO>> GetAllMaterias();
        public Task<MateriaDTO> GetMateria(long id);
        public Task<bool> EliminarMateria(long id);
        public Task<bool> EditarMateria(MateriaDTO materiaDTO);
        public Task<bool> CrearMateria(MateriaDTO materiaDTO);


    }
}
