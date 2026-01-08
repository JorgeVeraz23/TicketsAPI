using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class ParaleloRepository : IParalelo
    {

        private readonly ApplicationDbContext _context;
        public ParaleloRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // Crear paralelo
        public async Task<ParaleloResponseDto> CrearParaleloAsync(ParaleloDTO paraleloDto)
        {
            var paralelo = new Paralelo
            {
                Nombre = paraleloDto.Nombre,
                IsActive = true,  // Asignar IsActive como true
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Paralelos.Add(paralelo);
            await _context.SaveChangesAsync();

            return new ParaleloResponseDto
            {
                Id = paralelo.Id,
                Nombre = paralelo.Nombre
            };
        }

        // Obtener paralelo por ID
        public async Task<ParaleloResponseDto> ObtenerParaleloPorIdAsync(long id)
        {
            var paralelo = await _context.Paralelos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paralelo == null)
            {
                return null;
            }

            return new ParaleloResponseDto
            {
                Id = paralelo.Id,
                Nombre = paralelo.Nombre
            };
        }

        // Obtener todos los paralelos
        public async Task<List<ParaleloResponseDto>> ObtenerParalelosAsync()
        {
            var paralelos = await _context.Paralelos.Where(x => x.IsActive == true).ToListAsync();

            return paralelos.Select(p => new ParaleloResponseDto
            {
                Id = p.Id,
                Nombre = p.Nombre
            }).ToList();
        }

        // Actualizar paralelo
        public async Task<bool> ActualizarParaleloAsync(long id, ParaleloDTO paraleloDto)
        {
            var paralelo = await _context.Paralelos.FindAsync(id);

            if (paralelo == null)
            {
                return false;
            }

            paralelo.Nombre = paraleloDto.Nombre;
            paralelo.FechaModificacion = DateTime.UtcNow;
            paralelo.UsuarioModificacion = "SYSTEM";
            paralelo.IsActive = true;  // Asignar IsActive como true al actualizar

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar paralelo
        public async Task<bool> EliminarParaleloAsync(long id)
        {
            var paralelo = await _context.Paralelos.FindAsync(id);

            if (paralelo == null)
            {
                return false;
            }

            paralelo.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<KeyValueDTO>> SelectorParalelo()
        {
            var paralelos = await _context.Paralelos
                .Where(p => p.IsActive == true)
                .Select(p => new KeyValueDTO
                {
                    Key = p.Id,
                    Value = p.Nombre
                }).ToListAsync();

            return paralelos;
        }
    }
}
