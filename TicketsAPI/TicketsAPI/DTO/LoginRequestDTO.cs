namespace TicketsAPI.DTO
{
    public class LoginRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; } = "User";
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}

