using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class CalificacionRepository : ICalificacion
    {
        private readonly ApplicationDbContext _context;

        public CalificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> ActualizarAsync(long id, CalificacionCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<CalificacionResponseDto> CrearAsync(CalificacionCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CalificacionResponseDto>> ListarAsync(string? periodo, string? materia)
        {
            throw new NotImplementedException();
        }

        public Task<List<CalificacionResponseDto>> ListarPorEstudianteAsync(long estudianteId, string? periodo)
        {
            throw new NotImplementedException();
        }

        public Task<List<CalificacionResponseDto>> ListarPorProfesorAsync(long profesorId, string? periodo)
        {
            throw new NotImplementedException();
        }

        public Task<CalificacionResponseDto?> ObtenerPorIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
