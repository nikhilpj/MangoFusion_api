using Microsoft.AspNetCore.Identity;

namespace mango_fusion.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
