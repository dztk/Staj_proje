using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proje.Models;
using Proje.Services;
using System.Linq;
using System.Security.Claims;

namespace Proje.Controllers
{
    [Authorize]
    public class HatirlatmaController : BaseController
    {
        private readonly AppDbContext _context;
       // private readonly ReminderService _reminderService;

        public HatirlatmaController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
           // _reminderService = new ReminderService(context);
        }

        public IActionResult Index()
        {
            var kullaniciAdi = User.Identity.Name;
            var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullaniciAdi);

            string rol = User.IsInRole("Şef") ? "Şef" : "Teknisyen";
            int personelId = personel?.Id ?? 0;

            var hatirlatmalar = _reminderService.GetHatirlatmalar(rol, personelId);
            return View(hatirlatmalar);
        }
    }
}
