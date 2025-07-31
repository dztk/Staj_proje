using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Proje.Models;
using Proje.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ReminderService _reminderService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly AppDbContext _context;


        public BaseController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _reminderService = reminderService;
            _userManager = userManager;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Giriş yapılmamışsa boş bırak
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Hatirlatmalar = null;
                ViewBag.HatirlatmaSayisi = 0;
                return;
            }

            // Rol ve kullanıcı bilgisi al
            var rol = User.IsInRole("Şef") ? "Şef" : "Teknisyen";

           
            var userName = _userManager.GetUserName(User);
            var personelId = GetPersonelIdByUserName(userName);


            var hatirlatmalar = _reminderService.GetHatirlatmalar(rol, personelId);
            ViewBag.Hatirlatmalar = hatirlatmalar;
            ViewBag.HatirlatmaSayisi = hatirlatmalar.Count;
        }

        // KullaniciId → Personel tablosundaki Id'yi bulur
        private int GetPersonelIdByUserName(string userName)
        {
            
                var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == userName);
                return personel?.Id ?? 0;
            
        }
    }
}