using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class EstudianteRepository : IEstudiante
    {

        private readonly ApplicationDbContext _context;
        public EstudianteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearEstudiante(EstudianteDTO estudianteDTO)
        {
            Estudiantes estudiantes = new Estudiantes
            {
                Nombre = estudianteDTO.Nombre,
                Celular = estudianteDTO.Celular,
                Identificacion = estudianteDTO.Identificacion,
                FechaNacimiento = estudianteDTO.FechaNacimiento,
                RepresentanteId = estudianteDTO.RepresentanteId,
                Active = true,
                DateRegister = DateTime.UtcNow,
                IpRegister = "0.0.0.0",
                UserRegister = "SYSTEM",

            };

            await _context.Estudiantes.AddAsync(estudiantes);
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> EditarEstudiante(EstudianteDTO estudianteDTO)
        {
            var estudiante = await _context.Estudiantes.Where(x => x.IdEstudiantes == estudianteDTO.IdEstudiantes)
                .Select(x => new EstudianteDTO
                {
                    Nombre = x.Nombre,
                    Celular = x.Celular,
                    Edad = x.Edad,
                    FechaNacimiento = x.FechaNacimiento,
                    IdEstudiantes = x.IdEstudiantes,
                    Identificacion = x.Identificacion,
                    RepresentanteId = x.RepresentanteId,
                })
                .FirstOrDefaultAsync();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarEstudiante(long idEstudiante)
        {
            var representate = await _context.Estudiantes.FirstOrDefaultAsync(x => x.IdEstudiantes == idEstudiante) ?? throw new ArgumentNullException();

            representate.DateDelete = DateTime.UtcNow;
            representate.UserDelete = "SYSTEM";
            representate.IpDelete = "0.0.0.0";
            representate.Active = false;

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<List<EstudianteDTO>> GetAllEstudiante()
        {
            var estudianteList = await _context.Estudiantes.Where(x => x.Active)
                .Select(x => new EstudianteDTO {
                    Celular = x.Celular,
                    Nombre = x.Nombre,
                    FechaNacimiento = x.FechaNacimiento,
                    Identificacion = x.Identificacion,
                    Edad = x.Edad,
                    IdEstudiantes = x.IdEstudiantes,
                    RepresentanteId = x.RepresentanteId,
                })
                .ToListAsync();


            return estudianteList;


        }

        public async Task<EstudianteDTO> GetEstudianteById(long id)
        {
            var estudiante = await _context.Estudiantes.Where(x => x.IdEstudiantes == id)
                .Select(x => new EstudianteDTO
                {
                    Celular = x.Celular,
                    Nombre = x.Nombre,
                    IdEstudiantes = x.IdEstudiantes,
                    Edad = x.Edad,
                    FechaNacimiento = x.FechaNacimiento,
                    Identificacion = x.Identificacion,
                    RepresentanteId= x.RepresentanteId,
                })
                .FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            return estudiante;
        }
    }
}
