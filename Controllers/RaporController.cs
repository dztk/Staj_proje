using Microsoft.AspNetCore.Mvc;
using Proje.Models;
using Proje.Services;
using System.IO;

namespace Proje.Controllers
{
    public class RaporController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ToPdfService _pdfService;

        public RaporController(AppDbContext context, ToPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public IActionResult KapanisRaporu(int id)
        {
            var pdfBytes = _pdfService.CreateKapanisRaporuPdf(id);
            if (pdfBytes == null)
                return NotFound();

            return File(pdfBytes, "application/pdf", $"KapanisRaporu_{id}.pdf");
        }
    }
}