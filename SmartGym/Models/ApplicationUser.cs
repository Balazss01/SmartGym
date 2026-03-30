using Microsoft.AspNetCore.Identity;

namespace GymWebApiBackend.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? TeljesNev { get; set; }
    }
}
