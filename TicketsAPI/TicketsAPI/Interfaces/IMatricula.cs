using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IMatricula
    {
        public Task<MatriculaResponseDto> CrearMatriculaAsync(CrearMatriculaDto dto);


    }
}
