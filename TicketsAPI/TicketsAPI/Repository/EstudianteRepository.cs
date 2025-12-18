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

        // Crear estudiante
        public async Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteDTO estudianteDto)
        {
            try
            {
                var estudiante = new Estudiante
                {
                    Nombre = estudianteDto.Nombre,
                    Cedula = estudianteDto.Cedula,
                    Representante = estudianteDto.Representante,
                    Telefono = estudianteDto.Telefono,
                    Correo = estudianteDto.Correo,
                    Nivel = estudianteDto.Nivel,
                    IsActive = true,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = "SYSTEM"
                };

                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();

                // Mapear la entidad a DTO
                return new EstudianteResponseDto
                {
                    Id = estudiante.Id,
                    Nombre = estudiante.Nombre,
                    Cedula = estudiante.Cedula,
                    Representante = estudiante.Representante,
                    Telefono = estudiante.Telefono,
                    Correo = estudiante.Correo,
                    Nivel = estudiante.Nivel
                };
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                // Logger.LogError(ex, "Error al crear el estudiante");
                throw new Exception("Hubo un problema al crear el estudiante.", ex);
            }
        }

        // Obtener estudiante por ID
        public async Task<EstudianteResponseDto> ObtenerEstudiantePorIdAsync(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (estudiante == null)
                {
                    return null;
                }

                // Mapear la entidad a DTO
                return new EstudianteResponseDto
                {
                    Id = estudiante.Id,
                    Nombre = estudiante.Nombre,
                    Cedula = estudiante.Cedula,
                    Representante = estudiante.Representante,
                    Telefono = estudiante.Telefono,
                    Correo = estudiante.Correo,
                    Nivel = estudiante.Nivel
                };
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                // Logger.LogError(ex, "Error al obtener el estudiante por ID");
                throw new Exception($"Hubo un problema al obtener el estudiante con ID {id}.", ex);
            }
        }

        // Obtener todos los estudiantes
        public async Task<List<EstudianteResponseDto>> ObtenerTodosEstudiantesAsync()
        {
            try
            {
                var estudiantes = await _context.Estudiantes.ToListAsync();

                // Mapear las entidades a DTOs
                return estudiantes.Select(estudiante => new EstudianteResponseDto
                {
                    Id = estudiante.Id,
                    Nombre = estudiante.Nombre,
                    Cedula = estudiante.Cedula,
                    Representante = estudiante.Representante,
                    Telefono = estudiante.Telefono,
                    Correo = estudiante.Correo,
                    Nivel = estudiante.Nivel
                }).ToList();
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                // Logger.LogError(ex, "Error al obtener todos los estudiantes");
                throw new Exception("Hubo un problema al obtener todos los estudiantes.", ex);
            }
        }

        // Actualizar estudiante
        public async Task<bool> ActualizarEstudianteAsync(int id, EstudianteDTO estudianteDto)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);

                if (estudiante == null)
                {
                    return false;
                }

                estudiante.Nombre = estudianteDto.Nombre;
                estudiante.Cedula = estudianteDto.Cedula;
                estudiante.Representante = estudianteDto.Representante;
                estudiante.Telefono = estudianteDto.Telefono;
                estudiante.Correo = estudianteDto.Correo;
                estudiante.Nivel = estudianteDto.Nivel;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                // Logger.LogError(ex, "Error al actualizar el estudiante");
                throw new Exception($"Hubo un problema al actualizar el estudiante con ID {id}.", ex);
            }
        }

        // Eliminar estudiante
        public async Task<bool> EliminarEstudianteAsync(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);

                if (estudiante == null)
                {
                    return false;
                }

                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error
                // Logger.LogError(ex, "Error al eliminar el estudiante");
                throw new Exception($"Hubo un problema al eliminar el estudiante con ID {id}.", ex);
            }
        }
    }
}
