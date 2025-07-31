using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using Proje.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef")]
    public class AnalizController : BaseController
    {
        private readonly AppDbContext _context;

        public AnalizController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var aracBakimMaliyetleri = await _context.ParcaIsEmri
                .Include(p => p.IsEmri).ThenInclude(i => i.Arac)
                .Where(p => p.IsEmri.Arac != null)
                .GroupBy(p => p.IsEmri.Arac.Plaka)
                .Select(g => new
                {
                    Arac = g.Key,
                    ToplamMaliyet = g.Sum(p => p.Miktar * p.Parca.BirimFiyat)
                })
                .ToListAsync();

            var durakBakimMaliyetleri = await _context.ParcaIsEmri
                .Include(p => p.IsEmri).ThenInclude(i => i.Durak)
                .Where(p => p.IsEmri.Durak != null)
                .GroupBy(p => p.IsEmri.Durak.Ad)
                .Select(g => new
                {
                    Durak = g.Key,
                    ToplamMaliyet = g.Sum(p => p.Miktar * p.Parca.BirimFiyat)
                })
                .ToListAsync();

            var enCokArizaYapanArac = await _context.IsEmri
                .Where(i => i.AracId != null)
                .GroupBy(i => i.Arac.Plaka)
                .Select(g => new
                {
                    Arac = g.Key,
                    ArizaSayisi = g.Count()
                })
                .OrderByDescending(x => x.ArizaSayisi)
                .FirstOrDefaultAsync();

            var enCokArizaYapanDurak = await _context.IsEmri
                .Where(i => i.DurakId != null)
                .GroupBy(i => i.Durak.Ad)
                .Select(g => new
                {
                    Durak = g.Key,
                    ArizaSayisi = g.Count()
                })
                .OrderByDescending(x => x.ArizaSayisi)
                .FirstOrDefaultAsync();

            var enCokDegisenParca = await _context.ParcaIsEmri
                .Include(p => p.Parca)
                .GroupBy(p => p.Parca.Ad)
                .Select(g => new
                {
                    Parca = g.Key,
                    ToplamAdet = g.Sum(p => p.Miktar)
                })
                .OrderByDescending(x => x.ToplamAdet)
                .FirstOrDefaultAsync();

            ViewBag.AracBakimMaliyetleri = aracBakimMaliyetleri;
            ViewBag.DurakBakimMaliyetleri = durakBakimMaliyetleri;
            ViewBag.EnCokArizaArac = enCokArizaYapanArac;
            ViewBag.EnCokArizaDurak = enCokArizaYapanDurak;
            ViewBag.EnCokDegisenParca = enCokDegisenParca;

            return View();
        }
    }
}
