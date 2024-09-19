using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class FormFieldRepository : FormFieldInterface
    {

        private readonly ApplicationDbContext _context;
        MessageInfoSolicitudDTO infoSolicitudDTO = new MessageInfoSolicitudDTO();

        public FormFieldRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<MessageInfoSolicitudDTO> ActualizarFormField(FormFieldDTO formFieldDTO)
        {
            var model = await _context.FormFields.Where(x => x.Active && x.IdFormField == formFieldDTO.IdFormField).FirstOrDefaultAsync() ?? throw new ArgumentNullException("El form field ingresado fue nulo");


            model.Name = formFieldDTO.Name;
            model.Index = formFieldDTO.Index;
            model.IsOptional = formFieldDTO.IsOptional;
            model.FieldTypeId = formFieldDTO.FieldTypeId;
            model.FormGroupId = formFieldDTO.FormGroupId;


            await _context.SaveChangesAsync();

            infoSolicitudDTO.Cod = "201";
            infoSolicitudDTO.Mensaje = "Actualicion de formField exitosa";

            return infoSolicitudDTO;

        }

        public async Task<MessageInfoSolicitudDTO> CrearFormField(FormFieldDTO formFieldDTO)
        {
            FormField formField = new FormField
            {
                Name = formFieldDTO.Name,
                Index = formFieldDTO.Index,
                IsOptional = formFieldDTO.IsOptional,
                FieldTypeId = formFieldDTO.FieldTypeId,
                FormGroupId = formFieldDTO.FormGroupId,
                DateRegister = DateTime.Now,
                UserRegister = "SYSTEM",
                IpRegister = "::1",
                Active = true
            };

            await _context.AddAsync(formField);
            await _context.SaveChangesAsync();

            infoSolicitudDTO.Cod = "201";
            infoSolicitudDTO.Mensaje = "Form Field creado exitosamente";

            return infoSolicitudDTO;
        }

        public async Task<MessageInfoSolicitudDTO> EliminarFormField(long id)
        {
            var formFielIngresado = await _context.FormFields.Where(x => x.Active && x.IdFormField == id).FirstOrDefaultAsync() ?? throw new ArgumentNullException("el form field que se intenta eliminar no existe");

            formFielIngresado.Active = false;
            formFielIngresado.DateDelete = DateTime.Now;
            formFielIngresado.UserDelete = "SYSTEM";
            formFielIngresado.IpDelete = "::1";

            await _context.SaveChangesAsync();

            infoSolicitudDTO.Cod = "201";
            infoSolicitudDTO.Mensaje = "el form field a sido eliminado correctamente";

            return infoSolicitudDTO;


        }

        public async Task<FormFieldDTO> ObtenerFormField(long id)
        {
            var formFieldIngresado = await _context.FormFields.Where(x => x.Active && x.IdFormField == id).Select(c => new FormFieldDTO
            {
                IdFormField = c.IdFormField,
                Name = c.Name,
                Index = c.Index,
                IsOptional = c.IsOptional,
                FieldTypeId = c.FieldTypeId,
                FormGroupId = c.FormGroupId,
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException("El form fiel ingresado no existe");

            return formFieldIngresado;
        }

        public async Task<List<FormFieldDTO>> ObtenerTodosLosFormField()
        {
            var listaDeFormFields = await _context.FormFields.Where(x => x.Active).Select(c => new FormFieldDTO
            {
                IdFormField = c.IdFormField,
                Name = c.Name,
                Index = c.Index,
                IsOptional = c.IsOptional,
                FieldTypeId = c.FieldTypeId,
                FormGroupId = c.FormGroupId,
            }).ToListAsync();

            return listaDeFormFields;
        }

        public async Task<List<KeyValueDTO>> SelectorFormField()
        {
            var selectorFormField = await _context.FormFields.Where(x => x.Active).Select(c => new KeyValueDTO
            {
                Key = c.IdFormField,
                Value = c.Name,
            }).ToListAsync();

            return selectorFormField;
        }
    }
}
