using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class TipoDocumentoRepository : ITipoDocumento
    {
        private readonly ApplicationDbContext _context;
        public TipoDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<KeyValueDTO>> ListAsync()
        {
            var tiposDocumento = await _context.TipoDocumentos
                .Select(td => new KeyValueDTO
                {
                    Key = td.Id,
                    Value = td.Nombre
                })
                .ToListAsync();


            return tiposDocumento;
        }
    }
}
