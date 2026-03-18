using Microsoft.AspNetCore.Identity;

namespace GymWebApiBackend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? TeljesNev { get; set; }
    }
}
