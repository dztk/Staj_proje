using Microsoft.AspNetCore.Identity;

namespace Proje.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AdSoyad { get; set; }
        public string? Rol { get; set; } // Şef-Teknisyen  


    }
}


