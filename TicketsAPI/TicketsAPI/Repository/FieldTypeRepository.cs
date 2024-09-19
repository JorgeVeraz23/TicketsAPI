using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class FieldTypeRepository : FieldTypeInterface
    {
        private readonly ApplicationDbContext _context;
        MessageInfoSolicitudDTO infoDTO = new MessageInfoSolicitudDTO();

        public FieldTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageInfoSolicitudDTO> ActualizarFieldType(FieldTypeDTO fieldTypeDTO)
        {
            var model = await _context.FieldTypes.Where(x => x.Active && x.IdFieldType == fieldTypeDTO.IdFieldType).FirstOrDefaultAsync() ?? throw new ArgumentNullException("No existe el field type ingresado");

            model.Name = fieldTypeDTO.Name;

            model.DateModification = DateTime.Now;
            model.UserModification = "SYSTEM";
            model.IpModification = "::1";

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Field Type actualizado correctamente";

            return infoDTO;

        }

        public async Task<MessageInfoSolicitudDTO> CrearFieldType(FieldTypeDTO fieldTypeDTO)
        {
            try {
                FieldType fieldType = new FieldType();

                fieldType.Name = fieldTypeDTO.Name;
                fieldType.DateRegister = DateTime.Now;
                fieldType.UserRegister = "SYSTEM";
                fieldType.IpRegister = "::1";
                fieldType.Active = true;

                await _context.FieldTypes.AddAsync(fieldType);
                await _context.SaveChangesAsync();

                infoDTO.Cod = "201";
                infoDTO.Mensaje = "Field Type creado exitosamente";

                return infoDTO;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MessageInfoSolicitudDTO> EliminarFieldType(long id)
        {
            var fieldTypeToDelete = await _context.FieldTypes.Where(x => x.Active && x.IdFieldType == id).FirstOrDefaultAsync() ?? throw new ArgumentNullException("el field type ingresado es nulo");

            fieldTypeToDelete.Active = false;
            fieldTypeToDelete.DateDelete = DateTime.Now;
            fieldTypeToDelete.UserDelete = "SYSTEM";
            fieldTypeToDelete.IpDelete = "::1";

            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Fiel Type eliminado exitosamente";
           
            return infoDTO;
            
        }

        public async Task<FieldTypeDTO> MostrarFieldTypePorId(long id)
        {
            var FielTypeSeleccionado = await _context.FieldTypes.Where(x => x.Active && x.IdFieldType == id).Select(c => new FieldTypeDTO
            {
                IdFieldType = c.IdFieldType,
                Name = c.Name,
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException("El id field type no esta disponible");

            return FielTypeSeleccionado;
        }

        public async Task<List<FieldTypeDTO>> MostrarTodosLosFieldTypePorId()
        {
            var listaFieldType = await _context.FieldTypes.Where(x => x.Active).Select(c => new FieldTypeDTO
            {
                IdFieldType = c.IdFieldType,
                Name = c.Name,
            }).ToListAsync();

            return listaFieldType;
        }

        public async Task<List<KeyValueDTO>> SelectorFieldType()
        {
            var selectorFieldType = await _context.FieldTypes.Where(x => x.Active).Select(c => new KeyValueDTO
            {
                Key = c.IdFieldType,
                Value = c.Name,
            }).ToListAsync();

            return selectorFieldType;
        }
    }
}
