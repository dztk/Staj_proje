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
using ClosedXML.Excel;


namespace Proje.Controllers
{
    [Authorize(Roles = "Şef, Teknisyen")]
    public class ParcasController : BaseController
    {
        private readonly AppDbContext _context;

        public ParcasController(ReminderService reminderService, UserManager<ApplicationUser> userManager, AppDbContext context)
        : base(reminderService, userManager, context)
        {
            _context = context;
        }

        // GET: Parcas
        [Authorize(Roles = "Şef, Teknisyen")]
        public async Task<IActionResult> Index()
        {
            return _context.Parca != null ?
                        View(await _context.Parca.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Parca'  is null.");
        }

        // GET: Parcas/Details/5
        [Authorize(Roles = "Şef, Teknisyen")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Parca == null)
            {
                return NotFound();
            }

            var parca = await _context.Parca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parca == null)
            {
                return NotFound();
            }

            return View(parca);
        }

        // GET: Parcas/Create
        [Authorize(Roles = "Şef")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Parcas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,BirimFiyat,Stok")] Parca parca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parca);
        }

        // GET: Parcas/Edit/5
        [Authorize(Roles = "Şef")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Parca == null)
            {
                return NotFound();
            }

            var parca = await _context.Parca.FindAsync(id);
            if (parca == null)
            {
                return NotFound();
            }
            return View(parca);
        }

        // POST: Parcas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Şef")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,BirimFiyat,Stok")] Parca parca)
        {
            if (id != parca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParcaExists(parca.Id))
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
            return View(parca);
        }

        // GET: Parcas/Delete/5
        [Authorize(Roles = "Şef")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Parca == null)
            {
                return NotFound();
            }

            var parca = await _context.Parca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parca == null)
            {
                return NotFound();
            }

            return View(parca);
        }

        // POST: Parcas/Delete/5
        [Authorize(Roles = "Şef")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Parca == null)
            {
                return Problem("Entity set 'AppDbContext.Parca'  is null.");
            }
            var parca = await _context.Parca.FindAsync(id);
            if (parca != null)
            {
                _context.Parca.Remove(parca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Şef")]
        private bool ParcaExists(int id)
        {
            return (_context.Parca?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult ExportToExcel()
        {
            var parcalar = _context.Parca.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Parça Listesi");

                // Başlıklar
                worksheet.Cell(1, 1).Value = "Parça Adı";
                worksheet.Cell(1, 2).Value = "Birim Fiyatı";
                worksheet.Cell(1, 3).Value = "Stok Miktarı";

                int row = 2;
                foreach (var item in parcalar)
                {
                    worksheet.Cell(row, 1).Value = item.Ad;
                    worksheet.Cell(row, 2).Value = item.BirimFiyat;
                    worksheet.Cell(row, 3).Value = item.Stok;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ParcaRaporu.xlsx");
                }
            }
        }
    }
}
