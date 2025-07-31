using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Proje.Models;
using Proje.Services;
using System.Net.Http;
using Proje.Enums;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class DurakController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public DurakController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context, IHttpClientFactory clientFactory)
        : base(reminderService, userManager, context)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> GetDuraklarFromApi()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("https://mocki.io/v1/98b7b7b5-4514-48b4-a756-63d06a5654ad");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var duraklar = JsonConvert.DeserializeObject<List<Durak>>(json);

                if (!_context.Durak.Any())
                {
                    _context.Durak.AddRange(duraklar);
                    await _context.SaveChangesAsync();
                }

                return Ok(duraklar);
            }

            return StatusCode((int)response.StatusCode, "API isteği başarısız.");
        }

       
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == user.UserName);

            if (User.IsInRole("Şef"))
            {
                var duraklar = _context.Durak.OrderBy(d => d.Ad).ToList();
                return View(duraklar);
            }
            else if (User.IsInRole("Teknisyen") && personel != null)
            {
                var ilgiliDurakIds = _context.IsEmri
                    .Where(e => e.PersonelId == personel.Id && (e.Durum == IsEmriDurumu.Acik || e.Durum == IsEmriDurumu.Bekleme || e.Durum == IsEmriDurumu.Devam))
                    .Select(e => e.DurakId)
                    .Where(id => id != null)
                    .Distinct()
                    .ToList();

                var duraklar = _context.Durak
                    .Where(d => ilgiliDurakIds.Contains(d.Id))
                    .OrderBy(d => d.Ad)
                    .ToList();

                return View(duraklar);
            }

            return Forbid(); // Yetkisiz erişim
        }


        [Authorize(Roles = "Şef")]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [Authorize(Roles = "Şef")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Durak durak)
        {

            if (_context.Durak.Any(d => d.Ad == durak.Ad))
            {
                ModelState.AddModelError("", "Bu durak zaten mevcut.");
                return View(durak);
            }

            if (ModelState.IsValid)
            {
                _context.Add(durak);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(durak);
        }


        [Authorize(Roles = "Şef, Teknisyen")]
        public IActionResult Details(int id)
        {
            var kullaniciAdi = User.Identity.Name;
            var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullaniciAdi);

            if (User.IsInRole("Şef"))
            {
                var durak = _context.Durak.FirstOrDefault(x => x.Id == id);
                return durak == null ? NotFound() : View(durak);
            }

            if (User.IsInRole("Teknisyen") && personel != null)
            {
                // Teknisyene atanmış iş emirlerinden ilişkili durakları çekiyoruz
                var yetkiliDurakIdler = _context.IsEmri
                    .Where(e => e.PersonelId == personel.Id &&
                                e.Tip == Proje.Enums.IsEmriTipi.Durak &&
                                (e.Durum == IsEmriDurumu.Acik || e.Durum == IsEmriDurumu.Bekleme || e.Durum == IsEmriDurumu.Devam))
                    .Select(e => e.DurakId)
                    .ToList();

                if (yetkiliDurakIdler.Contains(id))
                {
                    var durak = _context.Durak.FirstOrDefault(x => x.Id == id);
                    return durak == null ? NotFound() : View(durak);
                }
                else
                {
                    return Forbid(); // Yetkisi yoksa engelle
                }
            }

            return Forbid(); // Eğer kullanıcı tanımsızsa da erişimi engelle
        }



        [Authorize(Roles = "Şef")]
        public IActionResult Edit(int id)
        {
            var durak = _context.Durak.FirstOrDefault(x => x.Id == id);
            return durak == null ? NotFound() : View(durak);
        }

        [HttpPost]
        [Authorize(Roles = "Şef")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Durak durak)
        {
            if (id != durak.Id) return NotFound();


            if (_context.Durak.Any(d => d.Ad == durak.Ad && d.Id != durak.Id))
            {
                ModelState.AddModelError("Ad", "Bu durak zaten mevcut.");
                return View(durak);
            }

            if (ModelState.IsValid)
            {
                _context.Update(durak);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(durak);
        }

        [Authorize(Roles = "Şef")]
        public IActionResult Delete(int id)
        {
            var durak = _context.Durak.FirstOrDefault(x => x.Id == id);
            return durak == null ? NotFound() : View(durak);
        }

        [Authorize(Roles = "Şef")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var durak = await _context.Durak.FindAsync(id);
            if (durak != null)
            {
                _context.Durak.Remove(durak);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}