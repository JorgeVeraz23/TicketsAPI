using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface SolicitudInterface
    {
        //ADMINISTRADOR
        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudesAdministrador();
        public Task<List<MostrarSolicitudDTO>> GetAllSolitudesByFilter(long idUsuario,  DateTime fechaIngreso);
        public Task<MostrarJustificativoDTO> GetJustificativo(long idUsuario);



        //CLIENTE
        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudes(long idUsuario);
        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudesByFilter(long idUsuario, DateTime FechaIngreso, string Estado);
        public Task<MessageInfoSolicitudDTO> CreateSolicitud(SolicitudDTO solicitud);
        public Task<SolicitudDTO> VerDetalleSolicitud(long idSolicitud);

    }

}
