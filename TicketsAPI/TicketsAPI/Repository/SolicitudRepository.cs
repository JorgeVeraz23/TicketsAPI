using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class SolicitudRepository : SolicitudInterface
    {
        private readonly ILogger<SolicitudRepository> _logger;

        private readonly ApplicationDbContext _context;

        public SolicitudRepository(ApplicationDbContext context, ILogger<SolicitudRepository> logger)
        {

            _context = context;
            _logger = logger;
        }
        public async Task<MessageInfoSolicitudDTO> CreateSolicitud(SolicitudDTO solicitud)
        {
            try
            {
                //Verifica si se subio un archivo
                string filePath = null;
                if(solicitud.Justificativo != null)
                {
                    //Guardar el archivo en el servidor
                    filePath = Path.Combine("uploads", solicitud.Justificativo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await solicitud.Justificativo.CopyToAsync(stream);
                    }
                }



                Solicitud solicitudEntity = new Solicitud
                {

                    tipoSolicitud = solicitud.tipoSolicitud,
                    DescripcionSolicitud = solicitud.DescripcionSolicitud,
                    Justificativo = filePath,
                    estadoSolicitud = solicitud.estadoSolicitud,
                    DetalleGestion = solicitud.DetalleGestion,
                    FechaIngreso = DateTime.Now,
                    IdUsuario = solicitud.IdUsuario,

                };

                await _context.Solicituds.AddAsync(solicitudEntity);
                await _context.SaveChangesAsync();

                

                return new MessageInfoSolicitudDTO
                {
                    Cod = "201",
                    Mensaje = "Se ha registrado la solicitud",
                };
            }catch(Exception ex) {
                _logger.LogError(ex, "Error al crear la solicitud con tipo {tipoSolicitud}" + solicitud.tipoSolicitud);
                return new MessageInfoSolicitudDTO { Mensaje  = "Error al intentar crear la solicitud", Cod = "500"};
            }
        }

        public Task<List<SolicitudDTO>> GetAllSolicitudes(long idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<List<SolicitudDTO>> GetAllSolicitudesByFilter(long idUsuario, DateTime FechaIngreso, string Estado)
        {
            throw new NotImplementedException();
        }

        public Task<List<SolicitudDTO>> GetAllSolictudes()
        {
            throw new NotImplementedException();
        }

        public Task<List<SolicitudDTO>> GetAllSolitudesByFilter(long idUsuario, DateTime fechaIngreso)
        {
            throw new NotImplementedException();
        }

        public Task<SolicitudDTO> GetJustificativo(long idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<SolicitudDTO> VerDetalleSolicitud(long idSolicitud)
        {
            throw new NotImplementedException();
        }
    }
}
