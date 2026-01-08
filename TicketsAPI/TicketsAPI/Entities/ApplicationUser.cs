using Microsoft.AspNetCore.Identity;

namespace TicketsAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}
