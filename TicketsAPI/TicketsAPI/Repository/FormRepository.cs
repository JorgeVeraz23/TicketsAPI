using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class FormRepository : FormInterface
    {
        
        private readonly ApplicationDbContext _context;
        MessageInfoSolicitudDTO infoDTO = new MessageInfoSolicitudDTO();
        public FormRepository(ApplicationDbContext context) {
            _context = context;
         
        }

        public async Task<MessageInfoSolicitudDTO> ActualizarFormulario(FormDTO formDTO)
        {
            var model = await _context.Forms.FirstOrDefaultAsync(x => x.IdForm == formDTO.IdForm) ?? throw new ArgumentNullException("Formulario ingresado no existe");

           
          
            model.Name = formDTO.Name;
            model.Description = formDTO.Description;

            model.DateModification = DateTime.Now;
            model.UserModification = "SYSTEM";
            model.IpModification = "::1";

            await _context.SaveChangesAsync();
            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Formulario actualizado correactamente";
          

            return infoDTO;

        }

        public async Task<MessageInfoSolicitudDTO> CrearFormulario(FormDTO formDTO)
        {
            // Crear una instancia de la entidad Form
            Form form = new Form();
            form.Name = formDTO.Name;
            form.Description = formDTO.Description;
            form.Active = true;
            form.DateRegister = DateTime.Now;
            

            // Agregar el nuevo formulario al contexto
            await _context.Forms.AddAsync(form);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Crear el mensaje de respuesta
            var message = new MessageInfoSolicitudDTO
            {
                Cod = "201",
                Mensaje = "Formulario creado exitosamente"
            };

            // Retornar el mensaje de respuesta
            return message;
        }

        public async Task<MessageInfoSolicitudDTO> ELiminarFormulario(long id)
        {
           
            var formularioBusqueda = await _context.Forms.Where(x => x.Active && x.IdForm == id).FirstOrDefaultAsync();

            if(formularioBusqueda == null)
            {
                infoDTO.Cod = "400";
                infoDTO.Mensaje = "No existe el formulario a eliminar";
            }

            formularioBusqueda.Active = false;
            formularioBusqueda.DateDelete = DateTime.Now;
            formularioBusqueda.UserDelete = "SYSTEM";
            formularioBusqueda.IpDelete = "::1";

            await _context.SaveChangesAsync();
            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Formulario eliminado correctamente";

            return infoDTO;
        }

        public async Task<List<KeyValueDTO>> KeyValueFormuario()
        {
            var selectorFormulario = await _context.Forms.Where(x => x.Active).Select(c => new KeyValueDTO
            {
                Key = c.IdForm,
                Value = c.Name,
            }).ToListAsync();

            return selectorFormulario;
        }

        public async Task<FormDTO> ObtenerFormularioPorId(long id)
        {
            var busqueda = await _context.Forms.Where(x => x.IdForm == id).Select(c => new FormDTO
            {
                IdForm = c.IdForm,
                Name = c.Name,
                Description = c.Description
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException("No hay resultados");



            if(busqueda == null)
            {
                infoDTO.Cod = "400";
                infoDTO.Mensaje = "No existe el formulario solicitado";

            }

            return busqueda;



        }

        public async Task<List<FormDTO>> ObtenerFormularios()
        {
            var listaDeFormularios = await _context.Forms.Where(x => x.Active).Select(c => new FormDTO
            {
                IdForm = c.IdForm,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();

            return listaDeFormularios;
        }
    }
}
