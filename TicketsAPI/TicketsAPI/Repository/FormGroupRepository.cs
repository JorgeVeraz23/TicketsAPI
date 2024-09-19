using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class FormGroupRepository : FormGroupInterface
    {
        private readonly ApplicationDbContext _context;
        MessageInfoSolicitudDTO infoDTO = new MessageInfoSolicitudDTO();

        public FormGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<MessageInfoSolicitudDTO> ActualizarFormulario(FormGroupDTO formGroupDTO)
        {
            var model = await _context.FormGroups.Where(x => x.Active && x.IdFormGroup == formGroupDTO.IdFormGroup).FirstOrDefaultAsync() ?? throw new ArgumentNullException(nameof(formGroupDTO)); 

            model.Name = formGroupDTO.Name;
            model.FormId = formGroupDTO.FormId;

            model.DateModification = DateTime.Now;
            model.UserModification = "SYSTEM";
            model.IpModification = "::ip";

            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Formulario actualizado correctamente!";

            return infoDTO;
            
        }

        public async Task<MessageInfoSolicitudDTO> CrearFormGroup(FormGroupDTO formGroupDTO)
        {
            FormGroup formGroup = new FormGroup
            {
                Name = formGroupDTO.Name,
                FormId = formGroupDTO.FormId,
                DateRegister = DateTime.Now,
                Active = true,
                UserRegister = "SYSTEM"
                
            };

            await _context.AddAsync(formGroup);
            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Form Group creado correctamente";

            return infoDTO;

            
        }

        public async Task<MessageInfoSolicitudDTO> EliminarFormGroup(long id)
        {
            var grupoToDelete = await _context.FormGroups.Where(x => x.Active && x.IdFormGroup == id).FirstOrDefaultAsync() ?? throw new ArgumentNullException("no existe el form group a eliminar");

            grupoToDelete.Active = false;
            grupoToDelete.UserDelete = "SYSTEM";
            grupoToDelete.DateDelete = DateTime.Now;
            grupoToDelete.IpDelete = "::1";

            await _context.SaveChangesAsync();

            infoDTO.Cod = "201";
            infoDTO.Mensaje = "Form Group eliminado correctamente";
            return infoDTO;


        }

        public async Task<List<KeyValueDTO>> KeyValueFormGroup()
        {
            var selector  = await _context.FormGroups.Where(x => x.Active).Select(x => new KeyValueDTO
            {
                Key = x.IdFormGroup,
                Value = x.Name
            }).ToListAsync();

            return selector;
        }

        public async Task<FormGroupDTO> ObtenerFormularioPorId(long id)
        {
            var formularioIngresado = await _context.FormGroups.Where(x => x.Active && x.IdFormGroup == id).Select(c => new FormGroupDTO
            {
                IdFormGroup = id,
                Name = c.Name,
                FormId = c.FormId
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException("No existe el formulario ingresado");

            return formularioIngresado;
            
        }

        public async Task<List<FormGroupDTO>> ObtenerTodosLosFormularios()
        {
            var listaDeGrupos = await _context.FormGroups.Where(x => x.Active).Select(c => new FormGroupDTO
            {
                IdFormGroup = c.IdFormGroup,
                Name = c.Name,
                FormId = c.FormId
            }).ToListAsync();

            return listaDeGrupos;
        }
    }
}
