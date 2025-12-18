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
                GradoNombre = materiaConGrado.Grado?.Nombre // Ahora debería tener el nombre del Grado
            };
        }

        // Obtener materia por ID
        public async Task<MateriaResponseDto> ObtenerMateriaPorIdAsync(int id)
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
                GradoNombre = materia.Grado.Nombre
            };
        }

        // Obtener todas las materias por grado
        public async Task<List<MateriaResponseDto>> ObtenerMateriasPorGradoAsync(int gradoId)
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
                GradoNombre = m.Grado.Nombre
            }).ToList();
        }

        // Actualizar materia
        public async Task<bool> ActualizarMateriaAsync(int id, MateriaDTO materiaDto)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return false;
            }

            materia.Nombre = materiaDto.Nombre;
            materia.GradoId = materiaDto.GradoId;

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar materia
        public async Task<bool> EliminarMateriaAsync(int id)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return false;
            }

            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
