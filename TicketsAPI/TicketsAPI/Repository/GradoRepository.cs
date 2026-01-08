using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class GradoRepository : IGrado
    {
        private readonly ApplicationDbContext _context;
        public GradoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<KeyValueDTO>> SelectorGrado()
        {
            var grados = await _context.Grados
                .Where(g => g.IsActive == true)
                .Select(g => new KeyValueDTO
                {
                    Key = g.Id,
                    Value = g.Nombre
                })
                .ToListAsync();


            return grados;
        }
    }
}
