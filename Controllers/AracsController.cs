using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje.Enums;
using Proje.Models;
using Proje.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class AracsController : BaseController
    {
        private readonly AppDbContext _context;

        public AracsController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context) 
        {
            _context = context;
        }

       
        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Index()
        {
            // Şu anki kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);

            // Şefse tüm araçlar gösterilir
            if (User.IsInRole("Şef"))
            {
                return View(await _context.Araclar
                    .OrderBy(a => a.Plaka)
                    .ToListAsync());
            }

            // Teknisyen ise sadece kendisine atanmış iş emirlerine bağlı araçları görebilir
            var personel = await _context.Personel
                .FirstOrDefaultAsync(p => p.KullaniciId == user.UserName);

            var aracIds = await _context.IsEmri
                .Where(e =>
                    e.PersonelId == personel.Id &&
                    (e.Durum == IsEmriDurumu.Acik || e.Durum == IsEmriDurumu.Bekleme || e.Durum == IsEmriDurumu.Devam) &&
                    e.AracId != null)
                .Select(e => e.AracId.Value)
                .Distinct()
                .ToListAsync();

            var araclar = await _context.Araclar
                .Where(a => aracIds.Contains(a.Id))
                .OrderBy(a => a.Plaka)
                .ToListAsync();

            return View(araclar);
        }


   
        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Araclar == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var arac = await _context.Araclar.FirstOrDefaultAsync(a => a.Id == id);

            if (arac == null)
                return NotFound();

            if (User.IsInRole("Şef"))
                return View(arac);

            var personel = await _context.Personel
                .FirstOrDefaultAsync(p => p.KullaniciId == user.UserName);

            var yetkiliMi = await _context.IsEmri.AnyAsync(e =>
                e.PersonelId == personel.Id &&
                (e.Durum == IsEmriDurumu.Acik || e.Durum == IsEmriDurumu.Bekleme || e.Durum == IsEmriDurumu.Devam) &&
                e.AracId == arac.Id);

            if (!yetkiliMi)
                return Forbid(); // Yetkisizse erişim yok

            return View(arac);
        }


        // GET: Aracs/Create
        [Authorize(Roles = "Şef, Teknisyen")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aracs/Create
        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Plaka,KapiNo,Marka,Model,Tip,KM")] Arac arac)
        {
            bool plakaVar = await _context.Araclar.AnyAsync(a => a.Plaka == arac.Plaka);
            bool kapiNoVar = await _context.Araclar.AnyAsync(a => a.KapiNo == arac.KapiNo);

            if (plakaVar)
                ModelState.AddModelError("Plaka", "Bu plaka zaten mevcut.");

            if (kapiNoVar)
                ModelState.AddModelError("KapiNo", "Bu kapı numarası zaten mevcut.");

            if (!ModelState.IsValid)
                return View(arac);

            _context.Add(arac);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Aracs/Edit/5
        [Authorize(Roles = "Şef")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Araclar == null)
                return NotFound();

            var arac = await _context.Araclar.FindAsync(id);
            if (arac == null)
                return NotFound();

            return View(arac);
        }

        // POST: Aracs/Edit/5
        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Plaka,KapiNo,Marka,Model,Tip,KM")] Arac arac)
        {
            if (id != arac.Id)
                return NotFound();

            bool plakaVar = await _context.Araclar.AnyAsync(a => a.Plaka == arac.Plaka && a.Id != arac.Id);
            bool kapiNoVar = await _context.Araclar.AnyAsync(a => a.KapiNo == arac.KapiNo && a.Id != arac.Id);

            if (plakaVar)
                ModelState.AddModelError("Plaka", "Bu plaka başka bir araca ait.");

            if (kapiNoVar)
                ModelState.AddModelError("KapiNo", "Bu kapı numarası başka bir araca ait.");

            if (!ModelState.IsValid)
                return View(arac);

            try
            {
                _context.Update(arac);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AracExists(arac.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Aracs/Delete/5
        [Authorize(Roles = "Şef")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Araclar == null)
                return NotFound();

            var arac = await _context.Araclar.FirstOrDefaultAsync(m => m.Id == id);
            if (arac == null)
                return NotFound();

            return View(arac);
        }

        // POST: Aracs/Delete/5
        [Authorize(Roles = "Şef")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Araclar == null)
                return Problem("Entity set 'AppDbContext.Araclar' is null.");

            var arac = await _context.Araclar.FindAsync(id);
            if (arac != null)
                _context.Araclar.Remove(arac);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AracExists(int id)
        {
            return (_context.Araclar?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}