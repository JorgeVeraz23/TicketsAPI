using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class AnioLectivoRepository : IAnioLectiivo
    {
        private readonly ApplicationDbContext _context;

        public AnioLectivoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearAnioLectivo(AnioLectivoDTO anioLectivoDTO)
        {
            var anioLectivo = new AnioLectivo
            {
                Periodo = anioLectivoDTO.Periodo,
                IsActive = true,  // Asignar IsActive como true
                FechaDesde = anioLectivoDTO.FechaDesde,
                FechaHasta = anioLectivoDTO.FechaHasta,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.AnioLectivo.Add(anioLectivo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditarAnioLectivo(AnioLectivoDTO anioLectivoDTO)
        {
            var anioLectivo = await _context.AnioLectivo
               .FirstOrDefaultAsync(p => p.Id == anioLectivoDTO.Id);

            if (anioLectivo == null)
            {
                return false;
            }

            anioLectivo.Periodo = anioLectivoDTO.Periodo;
            anioLectivo.FechaDesde = anioLectivoDTO.FechaDesde;
            anioLectivo.FechaHasta = anioLectivoDTO.FechaHasta;
            anioLectivo.FechaModificacion = DateTime.UtcNow;
            anioLectivo.UsuarioModificacion = "SYSTEM";

            await _context.SaveChangesAsync();



            return true;
        }

        public async Task<bool> EliminarAnioLectivo(long id)
        {
            var anioLectivo = await _context.AnioLectivo
               .FirstOrDefaultAsync(p => p.Id == id);

            if (anioLectivo == null)
            {
                return false;
            }

            anioLectivo.IsActive = false;
            anioLectivo.FechaEliminacion = DateTime.UtcNow;
            anioLectivo.UsuarioEliminacion = "SYSTEM";
            await _context.SaveChangesAsync();


            return true;

        }

        public Task<List<AnioLectivoDTO>> GetAllAnioLectivo()
        {
            var anioLectivoList = _context.AnioLectivo
                .Where(a => a.IsActive == true)
                .Select(a => new AnioLectivoDTO
                {
                    Id = a.Id,
                    Periodo = a.Periodo
                })
                .ToListAsync();
            
            return anioLectivoList;
        }

        public Task<AnioLectivoDTO> GetAnioLectivoById(long id)
        {
            var anioLectivo = _context.AnioLectivo
                .Where(a => a.Id == id && a.IsActive == true)
                .Select(a => new AnioLectivoDTO
                {
                    Id = a.Id,
                    Periodo = a.Periodo,
                    FechaDesde = a.FechaDesde,
                    FechaHasta = a.FechaHasta
                })
                .FirstOrDefaultAsync();


            return anioLectivo;
        }

        public Task<List<KeyValueDTO>> SelectorAnioLectivo()
        {
            var selectorList = _context.AnioLectivo
                .Where(a => a.IsActive == true)
                .Select(a => new KeyValueDTO
                {
                    Key = a.Id,
                    Value = a.Periodo
                })
                .ToListAsync();

            return selectorList;
        }
    }
}
