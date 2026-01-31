using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Services
{
    public class RepresentanteService : IRepresentanteService
    {
        private readonly IRepresentante _repo;
        private readonly ApplicationDbContext _db;

        public RepresentanteService(IRepresentante repo , ApplicationDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public Task<List<Representante>> ListAsync(string? q, CancellationToken ct)
            => _repo.ListAsync(q, ct);

        public async Task<Representante> GetAsync(long id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item ?? throw new KeyNotFoundException("Representante no existe.");
        }

        public async Task<long> CreateAsync(RepresentanteCreateDto dto, string userName, CancellationToken ct)
        {
            var exists = await _repo.ExistsByDocumentoAsync(dto.TipoDocumento, dto.NumeroDocumento, excludeId: null, ct);
            if (exists) throw new InvalidOperationException("Ya existe un representante con ese documento.");

            var entity = new Representante
            {
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                TipoDocumento = dto.TipoDocumento.Trim(),
                NumeroDocumento = dto.NumeroDocumento.Trim(),
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim(),

                IsActive = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = userName
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task<List<KeyValueDTO>> SelectorRepresentanteAsync()
        {
            return await _db.Representantes
                .AsNoTracking()
                .Where(r => r.IsActive == true)
                .OrderBy(r => r.Apellidos)
                .ThenBy(r => r.Nombres)
                .Select(r => new KeyValueDTO
                {
                    Key = r.Id,
                    Value = $"{r.Apellidos} {r.Nombres} - {r.NumeroDocumento}"
                })
                .ToListAsync();
        }


        public async Task UpdateAsync(long id, RepresentanteUpdateDto dto, string userName, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Representante no existe.");

            var exists = await _repo.ExistsByDocumentoAsync(dto.TipoDocumento, dto.NumeroDocumento, excludeId: id, ct);
            if (exists) throw new InvalidOperationException("Otro representante ya tiene ese documento.");

            entity.Nombres = dto.Nombres.Trim();
            entity.Apellidos = dto.Apellidos.Trim();
            entity.TipoDocumento = dto.TipoDocumento.Trim();
            entity.NumeroDocumento = dto.NumeroDocumento.Trim();
            entity.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            entity.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();

            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = userName;

            await _repo.UpdateAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(long id, string userName, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Representante no existe.");

            entity.IsActive = false;
            entity.FechaEliminacion = DateTime.UtcNow;
            entity.UsuarioEliminacion = userName;

            await _repo.UpdateAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);
        }
    }
}
