namespace TicketsAPI.DTO
{
    public class UsuarioDTO
    {
        public long IdUsuario { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public EnumRolUsuarioDTO Rol { get; set; }
    }
}
