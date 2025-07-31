using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje.Enums;
using Proje.Models;
using Proje.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class IsEmrisController : BaseController
    {
        private readonly AppDbContext _context;

        public IsEmrisController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Include(i => i.Personel)
                .OrderByDescending(x => x.AcilisTarihi)
                //.OrderByDescending(x => x.KapanisTarihi == null)  // Kapanmamışlar önce
                //.ThenByDescending(x => x.KapanisTarihi ?? x.AcilisTarihi) 
                .AsQueryable();

            if (User.IsInRole("Teknisyen"))
            {
                var kullaniciAdi = User.Identity.Name;
                var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == kullaniciAdi);
                query = personel != null ? query.Where(i => i.PersonelId == personel.Id) : query.Where(i => false);
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var isEmri = await _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Include(i => i.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            return isEmri == null ? NotFound() : View(isEmri);
        }

        [Authorize(Roles = "Şef")]
        public IActionResult Create()
        {
            ViewBag.Araclar = _context.Araclar
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Plaka} / {a.KapiNo}"
                }).ToList();

            ViewBag.Duraklar = _context.Durak
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.Ad} / {d.Kod}"
                }).ToList();

            ViewBag.Personeller = new SelectList(_context.Personel.Where(p => p.Unvan != "Şef"), "Id", "AdSoyad");

            ViewBag.Durumlar = Enum.GetValues(typeof(IsEmriDurumu))
                .Cast<IsEmriDurumu>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.GetType()
                            .GetMember(d.ToString())[0]
                            .GetCustomAttributes(typeof(DisplayAttribute), false)
                            .Cast<DisplayAttribute>()
                            .FirstOrDefault()?.Name ?? d.ToString()
                }).ToList();

            return View();
        }

        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IsEmri isEmri)
        {
            if (isEmri.PersonelId != null)
                isEmri.Durum = IsEmriDurumu.Bekleme;
            else
                isEmri.Durum = IsEmriDurumu.Acik;

            isEmri.AcilisTarihi = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(isEmri);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Araclar = _context.Araclar
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Plaka} - {a.KapiNo}"
                }).ToList();

            ViewBag.Duraklar = _context.Durak
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.Ad} - {d.Kod}"
                }).ToList();

            ViewBag.Personeller = new SelectList(_context.Personel.Where(p => p.Unvan != "Şef"), "Id", "AdSoyad", isEmri.PersonelId);
            return View(isEmri);
        }

        [Authorize(Roles = "Şef")]
        public IActionResult ImzaPdfYukle(int id)
        {
            var isEmri = _context.IsEmri.FirstOrDefault(x => x.Id == id);
            return isEmri == null ? NotFound() : View(isEmri);
        }

        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImzaPdfYukle(int id, IFormFile dosya)
        {
            var isEmri = await _context.IsEmri.FindAsync(id);
            if (isEmri == null) return NotFound();

            if (dosya != null && dosya.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfuploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                //  Eski dosyayı sil
                if (!string.IsNullOrEmpty(isEmri.ImzaPdf))
                {
                    var eskiDosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", isEmri.ImzaPdf.TrimStart('/'));
                    if (System.IO.File.Exists(eskiDosyaYolu))
                    {
                        System.IO.File.Delete(eskiDosyaYolu);
                    }
                }


                var fileName = $"imza_{id}_{Guid.NewGuid():N}.pdf";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dosya.CopyToAsync(stream);

                // Yeni yol veritabanına kaydedilir
                isEmri.ImzaPdf = $"/pdfuploads/{fileName}";

                _context.Update(isEmri);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var isEmri = await _context.IsEmri
                .Include(x => x.Personel)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (isEmri == null) return NotFound();

            if (isEmri.Durum == IsEmriDurumu.Kapali)
                return Forbid();

            var kullaniciAdi = User.Identity.Name;
            var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == kullaniciAdi);
            bool teknisyenMi = User.IsInRole("Teknisyen");

            if (teknisyenMi && isEmri.PersonelId != personel?.Id)
                return Unauthorized();

            ViewBag.TeknisyenMi = teknisyenMi;

            if (teknisyenMi)
            {
                ViewBag.Durumlar = new SelectList(new[] {
                    new { Value = (int)IsEmriDurumu.Devam, Text = "Devam" }
                }, "Value", "Text");
            }
            else
            {
                ViewBag.Personeller = new SelectList(
                    _context.Personel.Where(p => p.Unvan != "Şef"),
                    "Id", "AdSoyad", isEmri.PersonelId
                );
            }

            return View(isEmri);
        }

       
        [HttpPost]
        [Authorize(Roles = "Şef, Teknisyen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IsEmri isEmri)
        {
            if (id != isEmri.Id) return NotFound();

            var eskiIsEmri = await _context.IsEmri.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (eskiIsEmri == null) return NotFound();
            if (eskiIsEmri.Durum == IsEmriDurumu.Kapali)
                return Forbid();

            var kullaniciAdi = User.Identity.Name;
            var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == kullaniciAdi);
            bool teknisyenMi = User.IsInRole("Teknisyen");

            if (teknisyenMi)
            {
                if (eskiIsEmri.PersonelId != personel?.Id) return Unauthorized();

                if (eskiIsEmri.Durum == IsEmriDurumu.Bekleme && isEmri.Durum == IsEmriDurumu.Devam)
                {
                    var takipEdilenIsEmri = await _context.IsEmri.FirstOrDefaultAsync(x => x.Id == id);
                    takipEdilenIsEmri.Durum = IsEmriDurumu.Devam;

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // ŞEF kısmı
            var guncellenecekIsEmri = await _context.IsEmri.FirstOrDefaultAsync(x => x.Id == id);
            if (guncellenecekIsEmri == null) return NotFound();

            guncellenecekIsEmri.PersonelId = isEmri.PersonelId;
            guncellenecekIsEmri.Aciklama = isEmri.Aciklama;
            guncellenecekIsEmri.AracId = isEmri.AracId ?? eskiIsEmri.AracId;
            guncellenecekIsEmri.DurakId = isEmri.DurakId ?? eskiIsEmri.DurakId;
            guncellenecekIsEmri.AcilisTarihi = eskiIsEmri.AcilisTarihi;
            guncellenecekIsEmri.KapanisTarihi = eskiIsEmri.KapanisTarihi;


            if (eskiIsEmri.PersonelId != isEmri.PersonelId && isEmri.PersonelId != null)
            {
                guncellenecekIsEmri.Durum = IsEmriDurumu.Bekleme;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Şef")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var isEmri = await _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Include(i => i.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            return isEmri == null ? NotFound() : View(isEmri);
        }

        [Authorize(Roles = "Şef")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isEmri = await _context.IsEmri.FindAsync(id);
            if (isEmri != null)
            {
                _context.IsEmri.Remove(isEmri);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IsEmriExists(int id)
        {
            return _context.IsEmri.Any(e => e.Id == id);
        }
    }
}

