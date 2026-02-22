using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class MateriaRepository : IMateria
    {
        private readonly ApplicationDbContext _context;
        public MateriaRepository(ApplicationDbContext context)
        {
           _context = context; 
        }

        // Crear materia
        public async Task<MateriaResponseDto> CrearMateriaAsync(MateriaDTO materiaDto)
        {
            var materia = new Materia
            {
                Nombre = materiaDto.Nombre,
                GradoId = materiaDto.GradoId,
                IdProfesor = materiaDto.ProfesorId,
                IsActive = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            // Cargar el Grado relacionado explícitamente (Eager Loading)
            var materiaConGrado = await _context.Materias
                .Include(m => m.Grado)  // Carga explícitamente el Grado relacionado
                .FirstOrDefaultAsync(m => m.Id == materia.Id);

            // Mapear la entidad Materia a MateriaResponseDto
            return new MateriaResponseDto
            {
                Id = materiaConGrado.Id,
                Nombre = materiaConGrado.Nombre,
                GradoId = materiaConGrado.GradoId,
                GradoNombre = materiaConGrado.Grado?.Nombre, // Ahora debería tener el nombre del Grado
                ProfesorNombre = materiaConGrado.Profesor?.Nombres // Si también quieres incluir el nombre del Profesor
            };
        }

        // Obtener materia por ID
        public async Task<MateriaResponseDto> ObtenerMateriaPorIdAsync(long id)
        {
            var materia = await _context.Materias
                .Include(m => m.Grado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materia == null) return null;

            // Mapear la entidad Materia a MateriaResponseDto
            return new MateriaResponseDto
            {
                Id = materia.Id,
                Nombre = materia.Nombre,
                GradoId = materia.GradoId,
                GradoNombre = materia.Grado.Nombre,
                ProfesorNombre = materia.Profesor?.Nombres // Si también quieres incluir el nombre del Profesor
            };
        }

        // Obtener todas las materias por grado
        public async Task<List<MateriaResponseDto>> ObtenerMateriasPorGradoAsync(long gradoId)
        {
            var materias = await _context.Materias
                .Where(m => m.GradoId == gradoId)
                .Include(m => m.Grado)
                .ToListAsync();

            // Mapear las entidades Materia a MateriaResponseDto
            return materias.Select(m => new MateriaResponseDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                GradoId = m.GradoId,
                GradoNombre = m.Grado.Nombre,
                ProfesorNombre = m.Profesor?.Nombres // Si también quieres incluir el nombre del Profesor
            }).ToList();
        }

        // Actualizar materia
        public async Task<bool> ActualizarMateriaAsync(long id, MateriaDTO materiaDto)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return false;
            }

            materia.Nombre = materiaDto.Nombre;
            materia.GradoId = materiaDto.GradoId;
            materia.IdProfesor = materiaDto.ProfesorId;

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar materia
        public async Task<bool> EliminarMateriaAsync(long id)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return false;
            }

            materia.IsActive = false;
            materia.FechaEliminacion = DateTime.UtcNow;
            materia.UsuarioEliminacion = "SYSTEM";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MateriaResponseDto>> GetAllMaterias(long? idGrado)
        {
            var query = _context.Materias
                .AsNoTracking()
                .Where(m => m.IsActive == true);

            if (idGrado.HasValue)
                query = query.Where(m => m.GradoId == idGrado.Value);

            return await query
                .Select(m => new MateriaResponseDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    GradoId = m.GradoId,
                    GradoNombre = m.Grado.Nombre,
                    ProfesorId = m.IdProfesor,
                    ProfesorNombre = m.Profesor != null
                        ? m.Profesor.Nombres + " " + m.Profesor.Apellidos
                        : null
                })
                .ToListAsync();
        }


        public Task<List<KeyValueDTO>> SelectorMateria()
        {
            throw new NotImplementedException();
        }
    }
}
