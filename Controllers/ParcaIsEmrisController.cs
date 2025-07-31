using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje.Enums;
using Proje.Models;
using Proje.Services;

namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class ParcaIsEmrisController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParcaIsEmrisController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ParcaIsEmris
        public async Task<IActionResult> Index()
        {
            var query = _context.ParcaIsEmri
                .Include(t => t.Parca)
                .Include(t => t.IsEmri).ThenInclude(i => i.Arac)
                .Include(t => t.IsEmri).ThenInclude(i => i.Durak)
                .OrderByDescending(p => p.Id) 
                .AsQueryable();

            if (User.IsInRole("Teknisyen"))
            {
                var currentUsername = User.Identity.Name;
                var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == currentUsername);

                if (personel != null)
                    query = query.Where(p => p.PersonelId == personel.Id);
                else
                    query = query.Where(p => false);
            }

            return View(await query.ToListAsync());
        }

        // GET: ParcaIsEmris/Create
        [Authorize(Roles = "Teknisyen, Şef")]
        public async Task<IActionResult> Create()
        {
            ViewBag.ParcaId = new SelectList(_context.Parca, "Id", "Ad");

            List<SelectListItem> isEmriList = new();

            if (User.IsInRole("Teknisyen"))
            {
                var email = User.Identity.Name;
                var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == email);

                if (personel != null)
                {
                    isEmriList = await _context.IsEmri
                        //.Where(i => i.PersonelId == personel.Id && i.Durum != IsEmriDurumu.Kapali)
                        .Where(i => i.PersonelId == personel.Id && i.Durum == IsEmriDurumu.Devam)
                        .Include(i => i.Arac)
                        .Include(i => i.Durak)
                        .Select(i => new SelectListItem
                        {
                            Value = i.Id.ToString(),
                            Text = i.Tip == IsEmriTipi.Arac
                                ? $"Araç - {(i.Arac.Plaka ?? "Plaka Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                                : $"Durak - {(i.Durak.Ad ?? "Durak Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                        }).ToListAsync();
                }
            }
            else
            {
                // Şef ise tüm açık iş emirlerini görebilir
                isEmriList = await _context.IsEmri
                    .Where(i => i.Durum != IsEmriDurumu.Kapali)
                    .Include(i => i.Arac)
                    .Include(i => i.Durak)
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Tip == IsEmriTipi.Arac
                            ? $"Araç - {(i.Arac.Plaka ?? "Plaka Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                            : $"Durak - {(i.Durak.Ad ?? "Durak Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                    }).ToListAsync();
            }

            ViewBag.IsEmriId = isEmriList;
            return View();
        }

        // POST: ParcaIsEmris/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teknisyen, Şef")]
        public async Task<IActionResult> Create([Bind("Id,ParcaId,IsEmriId,Miktar,IscilikSuresiSaat")] ParcaIsEmri model)
        {
            var email = User.Identity.Name;
            var personel = await _context.Personel.FirstOrDefaultAsync(p => p.KullaniciId == email);
            if (personel != null)
            {
                model.PersonelId = personel.Id;
            }

            if (ModelState.IsValid)
            {
                var parca = await _context.Parca.FindAsync(model.ParcaId);
                if (parca == null)
                {
                    ModelState.AddModelError("", "Parça bulunamadı.");
                }
                else if (parca.Stok < model.Miktar)
                {
                    ModelState.AddModelError("Miktar", "Stok yetersiz.");
                }
                else
                {
                    parca.Stok -= model.Miktar;
                    _context.ParcaIsEmri.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.ParcaId = new SelectList(_context.Parca, "Id", "Ad", model.ParcaId);

            // Teknisyen sadece kendi iş emirlerini görsün
            List<SelectListItem> isEmriList = new();
            if (User.IsInRole("Teknisyen") && personel != null)
            {
                isEmriList = await _context.IsEmri
                    .Where(i => i.PersonelId == personel.Id && i.Durum != IsEmriDurumu.Kapali)
                    .Where(i => i.PersonelId == personel.Id && i.Durum == IsEmriDurumu.Devam)
                    .Include(i => i.Arac)
                    .Include(i => i.Durak)
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Tip == IsEmriTipi.Arac
                            ? $"Araç - {(i.Arac.Plaka ?? "Plaka Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                            : $"Durak - {(i.Durak.Ad ?? "Durak Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                    }).ToListAsync();
            }
            else
            {
                isEmriList = await _context.IsEmri
                    .Where(i => i.Durum != IsEmriDurumu.Kapali)
                    .Include(i => i.Arac)
                    .Include(i => i.Durak)
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Tip == IsEmriTipi.Arac
                            ? $"Araç - {(i.Arac.Plaka ?? "Plaka Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                            : $"Durak - {(i.Durak.Ad ?? "Durak Yok")} - {i.AcilisTarihi:dd.MM.yyyy}"
                    }).ToListAsync();
            }

            ViewBag.IsEmriId = isEmriList;

            return View(model);
        }

        // Diğer metotlar

        // GET: ParcaIsEmris/Details/5
        [Authorize(Roles = "Şef, Teknisyen")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) 
                return NotFound();

            var parcaIsEmri = await _context.ParcaIsEmri
                .Include(t => t.Parca)
                .Include(t => t.IsEmri)
                    .ThenInclude(i => i.Arac)
                .Include(t => t.IsEmri)
                    .ThenInclude(i => i.Durak)
                .Include(t => t.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (parcaIsEmri == null) return NotFound();

            return View(parcaIsEmri);
        }



        // GET: ParcaIsEmris/Edit/5
        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var parcaIsEmri = await _context.ParcaIsEmri.FindAsync(id);
            if (parcaIsEmri == null) return NotFound();

            ViewBag.ParcaId = new SelectList(_context.Parca, "Id", "Ad", parcaIsEmri.ParcaId);

            ViewBag.IsEmriId = _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Tip} - {(i.Tip == Enums.IsEmriTipi.Arac ? i.Arac.Plaka : i.Durak.Ad)}"
                }).ToList();

            return View(parcaIsEmri);
        }

        [Authorize(Roles = "Şef, Teknisyen")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ParcaIsEmri model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var mevcut = await _context.ParcaIsEmri
                        .Include(p => p.Parca) 
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (mevcut == null) return NotFound();

                    // Stok farkını güncelle
                    int eskiMiktar = mevcut.Miktar;
                    int yeniMiktar = model.Miktar;
                    int fark = eskiMiktar - yeniMiktar; 

                    var parca = await _context.Parca.FindAsync(mevcut.ParcaId);
                    if (parca == null) return NotFound();

                    parca.Stok += fark;

                    // Güncellenecek alanlar
                    mevcut.Miktar = yeniMiktar;
                    mevcut.IscilikSuresiSaat = model.IscilikSuresiSaat;

                    _context.Update(mevcut);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ParcaIsEmri.Any(e => e.Id == model.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.ParcaId = new SelectList(_context.Parca, "Id", "Ad", model.ParcaId);

            ViewBag.IsEmriId = _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Id} - {i.Tip} - {(i.Tip == Enums.IsEmriTipi.Arac ? i.Arac.Plaka : i.Durak.Ad)}"
                }).ToList();

            return View(model);
        }



        // GET: ParcaIsEmris/Delete/5
        [Authorize(Roles = "Teknisyen")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var parcaIsEmri = await _context.ParcaIsEmri
                .Include(t => t.Parca)
                .Include(t => t.IsEmri)
                    .ThenInclude(i => i.Arac)
                .Include(t => t.IsEmri)
                    .ThenInclude(i => i.Durak)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (parcaIsEmri == null) return NotFound();

            return View(parcaIsEmri);
        }

        // POST: ParcaIsEmris/Delete/5
        [Authorize(Roles = "Teknisyen")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ParcaIsEmri
               .Include(p => p.Parca)
               .FirstOrDefaultAsync(p => p.Id == id);

            if (model != null)
            {
                model.Parca.Stok += model.Miktar;

                _context.ParcaIsEmri.Remove(model);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}