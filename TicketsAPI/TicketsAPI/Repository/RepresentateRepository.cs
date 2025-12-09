using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class RepresentateRepository : IRepresentate
    {
        private readonly ApplicationDbContext _context;
        public RepresentateRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CrearRepresentate(RepresentanteDTO represetanteDTO)
        {
            Representante representate = new Representante
            {
                Nombre = represetanteDTO.Nombre,
                Edad = represetanteDTO.Edad,
                Identificacion = represetanteDTO.Identificacion,
                Celular = represetanteDTO.Celular,
                FechaNacimiento = represetanteDTO.FechaNacimiento,
                UserRegister = "SYSTEM",
                DateRegister = DateTime.UtcNow,
                IpRegister = "0.0.0.0"
            };

            await _context.Representante.AddAsync(representate);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRepresentate(long representanteId)
        {
            var representante = await _context.Representante.FirstOrDefaultAsync(x => x.IdRepresentante == representanteId);
            if (representante == null)
            {
                return false;
            }

            representante.Active = false;
            representante.IpDelete = "0.0.0.0";
            representante.UserDelete = "SYSTEM";
            representante.DateDelete = DateTime.UtcNow; 

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditarRepresentate(RepresentanteDTO representanteDTO)
        {
            var representante = await _context.Representante.Where(x => x.IdRepresentante == representanteDTO.IdRepresentante).FirstOrDefaultAsync() ;

            if(representante == null)
            {
                return false;
            }

            representante.Nombre = representanteDTO.Nombre;
            representante.Celular = representanteDTO.Celular;
            representante.Identificacion = representanteDTO.Identificacion;
            representante.FechaNacimiento = representanteDTO.FechaNacimiento;
            representante.DateModification = DateTime.UtcNow;
            representante.UserModification = "SYSTEM";
            representante.IpModification = "0.0.0.0";

            await _context.SaveChangesAsync();
            return true;
           


        }

        public async Task<List<RepresentanteDTO>> GetAllRepresentate()
        {
            var representanteList = await _context.Representante.Where(x => x.Active == true)
                .Select(x => new RepresentanteDTO
                {
                    Nombre = x.Nombre,
                    Celular = x.Celular,
                    Edad = x.Edad,
                    FechaNacimiento = x.FechaNacimiento,
                    Identificacion = x.Identificacion,
                    IdRepresentante = x.IdRepresentante,
                })
                .ToListAsync();

            return representanteList;

            


        }

        public async Task<RepresentanteDTO> GetRepresentateById(long representanteId)
        {
            var representante = await _context.Representante.Where(x => x.IdRepresentante == representanteId).Select(x => new RepresentanteDTO
            {
                IdRepresentante = x.IdRepresentante,
                Nombre = x.Nombre,
                Celular = x.Celular,
                Edad = x.Edad,
                Identificacion = x.Identificacion,
                FechaNacimiento = x.FechaNacimiento,
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            return representante;


        }
    }
}
