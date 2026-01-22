using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    // ----------------- DTOs (Single Role) -----------------

    public class CreateUserRequestDTO
    {
        [Required]
        public string Username { get; set; } = default!;

        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = default!;

        // ✅ UN SOLO ROL
        [Required]
        public string Role { get; set; } = default!;
    }

    public class UpdateUserRequestDTO
    {
        public string? Username { get; set; }

        // si mandas "" => null
        public string? Email { get; set; }

        public bool? IsActive { get; set; }

        // ✅ UN SOLO ROL
        public string? Role { get; set; }
    }

    public class ResetPasswordRequestDTO
    {
        [Required]
        public string NewPassword { get; set; } = default!;
    }


}
