namespace TicketsAPI.DTO.Auth
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // opcional: si no quieres permitir elegir rol desde el frontend, elimina esto
        public string? Role { get; set; }
    }
}
