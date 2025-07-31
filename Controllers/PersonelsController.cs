using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using Proje.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;


namespace Proje.Controllers
{
    [Authorize(Roles = "Şef")]
    public class PersonelsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PersonelsController(
    ReminderService reminderService,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    AppDbContext context)
    : base(reminderService, userManager, context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        // GET: Personels
        public async Task<IActionResult> Index()
        {
            return _context.Personel != null ?
                        View(await _context.Personel.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Personel'  is null.");
        }

        // GET: Personels/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Personel == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel

                .FirstOrDefaultAsync(m => m.Id == id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);

        }


        public IActionResult Create()
        {
            return View();
        }

        // POST: Personels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,Unvan,KullaniciId")] Personel personel)
        {

            if (ModelState.IsValid)
            {
                // KullaniciId benzersiz mi kontrol et
                if (_context.Personel.Any(p => p.KullaniciId == personel.KullaniciId))
                {
                    ModelState.AddModelError("KullaniciId", "Bu email zaten bir personelde kullanılıyor.");
                    return View(personel);
                }

                var yeniKullanici = new ApplicationUser
                {
                    UserName = personel.KullaniciId,
                    Email = personel.KullaniciId,
                    EmailConfirmed = true
                };

                var sonuc = await _userManager.CreateAsync(yeniKullanici, "Gecici123!");
                if (!sonuc.Succeeded)
                {
                    foreach (var hata in sonuc.Errors)
                    {
                        ModelState.AddModelError("", hata.Description);
                    }
                    return View(personel);
                }

               
                if (!await _roleManager.RoleExistsAsync(personel.Unvan))
                {
                    await _roleManager.CreateAsync(new IdentityRole(personel.Unvan));
                }

                await _userManager.AddToRoleAsync(yeniKullanici, personel.Unvan);

                
                _context.Add(personel);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(personel);


        }


        // GET: Personels/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Personel == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel.FindAsync(id);
            if (personel == null)
            {
                return NotFound();
            }
            return View(personel);
        }

       
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,Unvan,KullaniciId")] Personel guncelPersonel)
        {
            if (id != guncelPersonel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var mevcutPersonel = await _context.Personel.FindAsync(id);
                if (mevcutPersonel == null)
                {
                    return NotFound();
                }

                // Identity'deki kullanıcıyı bul
                var user = await _userManager.FindByNameAsync(mevcutPersonel.KullaniciId);
                if (user == null)
                {
                    return NotFound("Kullanıcı Identity sisteminde bulunamadı.");
                }

                // Eski rolü al
                var eskiRoller = await _userManager.GetRolesAsync(user);

                mevcutPersonel.AdSoyad = guncelPersonel.AdSoyad;
                mevcutPersonel.Unvan = guncelPersonel.Unvan;

                try
                {
                    _context.Update(mevcutPersonel);
                    await _context.SaveChangesAsync();

                   
                    if (!eskiRoller.Contains(guncelPersonel.Unvan))
                    {
                        await _userManager.RemoveFromRolesAsync(user, eskiRoller);
                        if (!await _roleManager.RoleExistsAsync(guncelPersonel.Unvan))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(guncelPersonel.Unvan));
                        }
                        await _userManager.AddToRoleAsync(user, guncelPersonel.Unvan);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonelExists(guncelPersonel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(guncelPersonel);
        }

        // GET: Personels/Delete/5


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Personel == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Personel == null)
            {
                return Problem("Entity set 'AppDbContext.Personel' is null.");
            }

            var personel = await _context.Personel.FindAsync(id);
            if (personel != null)
            {
                var identityUser = await _userManager.FindByNameAsync(personel.KullaniciId);

             
                if (identityUser != null)
                {
                    await _userManager.DeleteAsync(identityUser);
                }

                _context.Personel.Remove(personel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool PersonelExists(int id)
        {
            return _context.Personel.Any(e => e.Id == id);
        }

    }
}


