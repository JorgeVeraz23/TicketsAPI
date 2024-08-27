namespace TicketsAPI.DTO
{
    public class MessageInfoDTO
    {
        public bool IsValid { get; set; }
        public EnumRolUsuarioDTO Rol { get; set; }
    }

    public class MessageInfoSolicitudDTO{
        public string Cod { get; set; }
        public string Mensaje { get; set; }
    }
}
