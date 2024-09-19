using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class OptionRepository : OptionInterface
    {

        private readonly ApplicationDbContext _context;
        MessageInfoSolicitudDTO infoDTO = new MessageInfoSolicitudDTO();

        public OptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<MessageInfoSolicitudDTO> ActualizarOptionDTO(OptionDTO optionDTO)
        {
            var model = await _context.Options.Where(x => x.IdOption == optionDTO.IdOption).FirstOrDefaultAsync() ?? throw new ArgumentNullException("La opcion ingresada no existe");

            model.Name = optionDTO.Name;
            model.DateModification = DateTime.Now;
            model.UserModification = "SYSTEMS";
            model.IpModification = "::1";

            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Opción actualizada correctamente";

            return infoDTO;
        }

        public async Task<MessageInfoSolicitudDTO> CrearOpcionDTO(OptionDTO optionDTO)
        {
            Option option = new Option
            {
                Name = optionDTO.Name,
                idFormField = optionDTO.idFormField,
            };

            await _context.AddAsync(option);
            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Opción creada exitosamente";


            return infoDTO;
        }

        public async Task<MessageInfoSolicitudDTO> EliminarOptionDTO(long id)
        {
            var optionToDelete = await _context.Options.Where(x => x.Active && x.IdOption == id).FirstOrDefaultAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Opción Eliminada exitosamente";
            return infoDTO;
        }

        public async Task<OptionDTO> ObtenerOpcionPorId(long id)
        {
            var opcionIngresada = await _context.Options.Where(x => x.Active && x.IdOption == id).Select(c => new OptionDTO
            {
                IdOption = c.IdOption,
                idFormField = c.idFormField,
                Name = c.Name
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException("La opcion ingresada no existe");


            return opcionIngresada;
        }

        public async Task<List<OptionDTO>> ObtenerTodasLasOpciones()
        {
            var listaDeOpciones = await _context.Options.Where(x => x.Active).Select(c => new OptionDTO
            {
                IdOption = c.IdOption,
                Name = c.Name,
                idFormField = c.idFormField,
            }).ToListAsync();

            return listaDeOpciones;
        }

        public async Task<List<KeyValueDTO>> SelectorOptionDTO()
        {
            var opcionesSelector = await _context.Options.Where(x => x.Active).Select(c => new KeyValueDTO
            {
                Key = c.IdOption,
                Value = c.Name,
            }).ToListAsync();

            return opcionesSelector;
        }
    }
}
