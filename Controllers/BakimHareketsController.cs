using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using Proje.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class BakimHareketsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BakimHareketsController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BakimHarekets
        public async Task<IActionResult> Index()
        {
            var query = _context.BakimHareket
                .Include(b => b.IsEmri)
                    .ThenInclude(i => i.Arac)
                .Include(b => b.IsEmri)
                    .ThenInclude(i => i.Durak)
                .Include(b => b.Personel)
                .OrderByDescending(x => x.Tarih)
                .AsQueryable();

            if (User.IsInRole("Teknisyen"))
            {
                var email = User.Identity.Name;
                var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == email);

                if (personel != null)
                {
                    query = query.Where(b => b.PersonelId == personel.Id);
                }
                else
                {
                    query = query.Where(b => false); // Kayıt yoksa veri gösterme
                }
            }

            return View(await query.ToListAsync());
        }

        // GET: BakimHarekets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BakimHareket == null)
                return NotFound();

            var bakimHareket = await _context.BakimHareket
                .Include(b => b.IsEmri)
                    .ThenInclude(i => i.Arac)
                .Include(b => b.IsEmri)
                    .ThenInclude(i => i.Durak)
                .Include(b => b.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bakimHareket == null)
                return NotFound();

            return View(bakimHareket);
        }

        // GET: BakimHarekets/Create
        [Authorize(Roles = "Teknisyen")]
        public async Task<IActionResult> Create()
        {
            var email = User.Identity.Name;
            var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == email);

            if (personel == null)
                return BadRequest("Kullanıcı için kayıtlı iş emri bulunamadı.");

            var isEmriList = await _context.IsEmri
                .Where(i => i.PersonelId == personel.Id &&
                            i.Durum == Proje.Enums.IsEmriDurumu.Devam)

                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .ToListAsync();

            ViewBag.IsEmriId = isEmriList.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Tip == Proje.Enums.IsEmriTipi.Arac
                    ? $"Araç - {i.Arac.Plaka} / {i.Arac.KapiNo}"
                    : $"Durak - {i.Durak.Ad} / {i.Durak.Kod}"
            }).ToList();

            return View();
        }

        // POST: BakimHarekets/Create
        [Authorize(Roles = "Teknisyen")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsEmriId,YapilanIslem")] BakimHareket bakimHareket)
        {
            var email = User.Identity.Name;
            var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == email);

            if (personel == null)
                return BadRequest("Kullanıcı için personel kaydı bulunamadı.");

            if (ModelState.IsValid)
            {
                bakimHareket.Tarih = DateTime.Now;
                bakimHareket.PersonelId = personel.Id;

                _context.Add(bakimHareket);

                var isEmri = await _context.IsEmri.FindAsync(bakimHareket.IsEmriId);
                if (isEmri != null)
                {
                    isEmri.Durum = Proje.Enums.IsEmriDurumu.Kapali;
                    isEmri.KapanisTarihi = DateTime.Now;
                    _context.Update(isEmri);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            var isEmriList = await _context.IsEmri
                .Where(i => i.PersonelId == personel.Id &&
                            i.Durum == Proje.Enums.IsEmriDurumu.Devam)
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .ToListAsync();

            ViewBag.IsEmriId = isEmriList.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Tip == Proje.Enums.IsEmriTipi.Arac
                    ? $"Araç - {i.Arac.Plaka} / {i.Arac.KapiNo}"
                    : $"Durak - {i.Durak.Ad} / {i.Durak.Kod}"
            }).ToList();

            return View(bakimHareket);
        }


        // GET: Edit
        [Authorize(Roles = "Teknisyen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bakimHareket = await _context.BakimHareket.FindAsync(id);
            if (bakimHareket == null) return NotFound();

            ViewBag.IsEmriId = new SelectList(_context.IsEmri, "Id", "Id", bakimHareket.IsEmriId);
            return View(bakimHareket);
        }

        // POST: Edit
        [Authorize(Roles = "Teknisyen")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("YapilanIslem")] BakimHareket bakimHareket)   
        {
            if (id != bakimHareket.Id) return NotFound();

            var mevcut = await _context.BakimHareket.FindAsync(id);
            if (mevcut == null) return NotFound();

            mevcut.YapilanIslem = bakimHareket.YapilanIslem;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bakimHareket = await _context.BakimHareket
                .Include(b => b.IsEmri).ThenInclude(i => i.Arac)
                .Include(b => b.IsEmri).ThenInclude(i => i.Durak)
                .Include(b => b.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bakimHareket == null) return NotFound();

            return View(bakimHareket);
        }

        // POST: Delete
        [Authorize(Roles = "Şef, Teknisyen")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bakimHareket = await _context.BakimHareket.FindAsync(id);
            if (bakimHareket != null)
            {
                _context.BakimHareket.Remove(bakimHareket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
