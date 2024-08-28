using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface SolicitudInterface
    {
        //ADMINISTRADOR
        //Realizado
        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudesAdministrador();
        //Realizado
        public Task<List<MostrarSolicitudAdministradorDTO>> GetAllSolitudesByFilter(long idUsuario,  DateTime fechaIngreso);
        //Realizado
        public Task<List<MostrarJustificativoDTO>> GetJustificativo(long idSolicitud);
        
        public Task<MessageInfoSolicitudDTO> ActualizarSolicitud(ActualizarSolicitudDTO solicitud);
        



        //CLIENTE

        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudes(long idUsuario);
        public Task<List<MostrarSolicitudDTO>> GetAllSolicitudesByFilterCliente(long idUsuario, DateTime FechaIngreso, EnumEstadoSolicitud Estado);
        public Task<MessageInfoSolicitudDTO> CreateSolicitud(SolicitudDTO solicitud);
        public Task<MostrarSolicitudAdministradorDTO> VerDetalleSolicitud(long idSolicitud);

    }

}
