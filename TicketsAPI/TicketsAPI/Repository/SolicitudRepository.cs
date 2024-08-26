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
        public async Task<bool> CreateSolicitud(SolicitudDTO solicitud)
        {
            try
            {
                Solicitud solicitudEntity = new Solicitud
                {

                    tipoSolicitud = solicitud.tipoSolicitud,
                    DescripcionSolicitud = solicitud.DescripcionSolicitud,
                    Justificativo = solicitud.Justificativo,
                    estadoSolicitud = solicitud.estadoSolicitud,
                    DetalleGestion = solicitud.DetalleGestion,
                    FechaIngreso = DateTime.Now,
                    FechaGestion = solicitud.FechaGestion,
                    IdUsuario = solicitud.IdUsuario,

                };

                await _context.Solicituds.AddAsync(solicitudEntity);
                await _context.SaveChangesAsync();

                return true;
            }catch(Exception ex) {
                _logger.LogError(ex, "Error al crear la solicitud con tipo {tipoSolicitud}" + solicitud.tipoSolicitud);
                return false;
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
