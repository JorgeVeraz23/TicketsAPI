using Microsoft.EntityFrameworkCore;
using System;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class RepresentanteRepository : IRepresentante
    {
        private readonly ApplicationDbContext _db;
        public RepresentanteRepository(ApplicationDbContext db) => _db = db;

        public async Task<List<Representante>> ListAsync(string? q, CancellationToken ct)
        {
            var query = _db.Set<Representante>()
                .AsNoTracking()
                .Where(x => x.IsActive == true);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.Nombres.Contains(q) ||
                    x.Apellidos.Contains(q) ||
                    x.NumeroDocumento.Contains(q) ||
                    (x.Email != null && x.Email.Contains(q)) ||
                    (x.Telefono != null && x.Telefono.Contains(q)));
            }

            return await query
                .OrderBy(x => x.Apellidos)
                .ThenBy(x => x.Nombres)
                .ToListAsync(ct);
        }

        public Task<Representante?> GetByIdAsync(long id, CancellationToken ct)
        {
            return _db.Set<Representante>()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true, ct);
        }

    

        public Task<bool> ExistsByDocumentoAsync(string tipoDocumento, string numeroDocumento, long? excludeId, CancellationToken ct)
        {
            tipoDocumento = tipoDocumento.Trim();
            numeroDocumento = numeroDocumento.Trim();

            var query = _db.Set<Representante>().AsQueryable()
                .Where(x => x.IsActive == true &&
                            x.TipoDocumento == tipoDocumento &&
                            x.NumeroDocumento == numeroDocumento);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return query.AnyAsync(ct);
        }

        public Task AddAsync(Representante entity, CancellationToken ct)
        {
            _db.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Representante entity, CancellationToken ct)
        {
            _db.Update(entity);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
