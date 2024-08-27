using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface SolicitudInterface
    {
        //ADMINISTRADOR
        public Task<List<SolicitudDTO>> GetAllSolictudes();
        public Task<List<SolicitudDTO>> GetAllSolitudesByFilter(long idUsuario,  DateTime fechaIngreso);
        public Task<SolicitudDTO> GetJustificativo(long idUsuario);



        //CLIENTE
        public Task<List<SolicitudDTO>> GetAllSolicitudes(long idUsuario);
        public Task<List<SolicitudDTO>> GetAllSolicitudesByFilter(long idUsuario, DateTime FechaIngreso, string Estado);
        public Task<MessageInfoSolicitudDTO> CreateSolicitud(SolicitudDTO solicitud);
        public Task<SolicitudDTO> VerDetalleSolicitud(long idSolicitud);

    }

}
