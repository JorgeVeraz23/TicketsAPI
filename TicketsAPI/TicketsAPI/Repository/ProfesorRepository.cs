using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class ProfesorRepository : IProfesor
    {

        private readonly ApplicationDbContext _context;

        public ProfesorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearProfesor(ProfesorDTO profesorDTO)
        {
            var profesor = new Profesor
            {
                Nombre = profesorDTO.Nombre,
                Celular = profesorDTO.Celular,
                Edad = profesorDTO.Edad,
                FechaNacimiento = profesorDTO.FechaNacimiento,
                Identificacion = profesorDTO.Identificacion,
                DateRegister = DateTime.UtcNow,
                UserRegister = "SYSTEM",
                IpRegister = "0.0.0.0"
            };

            await _context.Profesor.AddAsync(profesor);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditarProfesor(ProfesorDTO profesorDTO)
        {
            var profesor = await _context.Profesor.Where(x => x.IdProfesor == profesorDTO.IdProfesor).FirstOrDefaultAsync();

            if (profesor == null) {
                return false;
            }

            profesor.Nombre = profesorDTO.Nombre;
            profesor.Identificacion = profesorDTO.Identificacion;
            profesor.Celular = profesorDTO.Celular;
            profesor.FechaNacimiento = profesorDTO.FechaNacimiento;
            profesor.Edad = profesorDTO.Edad;
            profesor.DateModification = DateTime.UtcNow;
            profesor.UserModification = "SYSTEM";
            profesor.IpModification = "0.0.0.0";


            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<bool> EliminarProfesor(long idProfesor)
        {
            var profesor = await _context.Profesor.Where(x => x.IdProfesor == idProfesor).FirstOrDefaultAsync();

            if(profesor == null)
            {
                return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProfesorDTO>> GetAllProfesor()
        {

            var profesorList = await _context.Profesor.Where(x => x.Active == true).Select(x => new ProfesorDTO
            {
                Nombre = x.Nombre,
                Celular = x.Celular,
                FechaNacimiento = x.FechaNacimiento,
                Edad = x.Edad,
                Identificacion = x.Identificacion,
                IdProfesor = x.IdProfesor,
            }).ToListAsync();

            return profesorList;



        }

        public async Task<ProfesorDTO> GetProfesorById(long idProfesor)
        {
            var profesor = await _context.Profesor.Where(x => x.IdProfesor == idProfesor).Select(x => new ProfesorDTO
            {
                Nombre = x.Nombre,
                Celular = x.Celular,
                Edad = x.Edad,
                FechaNacimiento = x.FechaNacimiento,
                Identificacion = x.Identificacion,
                IdProfesor = x.IdProfesor,
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            return profesor;
        }
    }
}
