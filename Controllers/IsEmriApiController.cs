using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using Proje.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Proje.Dto;


namespace Proje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class IsEmriApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IsEmriApiController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<IsEmriDto>>> GetIsEmri()
        {
            var isEmirleri = await _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Include(i => i.Personel)
                .ToListAsync();

            var dtoList = isEmirleri.Select(i => new IsEmriDto
            {
                Id = i.Id,
                Tip = i.Tip.ToString(),
                AracPlaka = i.Arac?.Plaka,
                DurakAd = i.Durak?.Ad,
                PersonelAdSoyad = i.Personel?.AdSoyad,
                Aciklama = i.Aciklama,
                Durum = i.Durum.ToString(),
                AcilisTarihi = i.AcilisTarihi
            });

            return Ok(dtoList);
        }


        
        [HttpGet("{id}")]
        public async Task<ActionResult<IsEmriDto>> GetIsEmri(int id)
        {
            var isEmri = await _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .Include(i => i.Personel)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (isEmri == null)
            {
                return NotFound();
            }

            var dto = new IsEmriDto
            {
                Id = isEmri.Id,
                Tip = isEmri.Tip.ToString(),
                AracPlaka = isEmri.Arac?.Plaka,
                DurakAd = isEmri.Durak?.Ad,
                PersonelAdSoyad = isEmri.Personel?.AdSoyad,
                Aciklama = isEmri.Aciklama,
                Durum = isEmri.Durum.ToString(),
                AcilisTarihi = isEmri.AcilisTarihi
            };

            return Ok(dto);
        }

        // POST: api/IsEmriApi
        [HttpPost]
        public async Task<ActionResult<IsEmri>> PostIsEmri(IsEmri isEmri)
        {
            
            isEmri.AcilisTarihi = DateTime.Now;
            isEmri.Durum = isEmri.PersonelId != null ? IsEmriDurumu.Bekleme : IsEmriDurumu.Acik;

            _context.IsEmri.Add(isEmri);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIsEmri), new { id = isEmri.Id }, isEmri);
        }

        // PUT: api/IsEmriApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIsEmri(int id, IsEmri isEmri)
        {
            if (id != isEmri.Id)
            {
                return BadRequest();
            }

            var mevcutIsEmri = await _context.IsEmri.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (mevcutIsEmri == null)
            {
                return NotFound();
            }

            // Sistem kontrol alanları
            isEmri.AcilisTarihi = mevcutIsEmri.AcilisTarihi;
            isEmri.KapanisTarihi = mevcutIsEmri.KapanisTarihi;
            isEmri.Durum = mevcutIsEmri.Durum;

            _context.Entry(isEmri).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.IsEmri.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/IsEmriApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIsEmri(int id)
        {
            var isEmri = await _context.IsEmri.FindAsync(id);
            if (isEmri == null)
            {
                return NotFound();
            }

            _context.IsEmri.Remove(isEmri);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonElement data)
        {
            var isEmri = await _context.IsEmri.FindAsync(id);
            if (isEmri == null)
            {
                return NotFound();
            }

            foreach (var prop in data.EnumerateObject())
            {
                switch (prop.Name.ToLower())
                {
                    case "aciklama":
                        isEmri.Aciklama = prop.Value.GetString();
                        break;
                    case "personelid":
                        isEmri.PersonelId = prop.Value.ValueKind == JsonValueKind.Null ? null : prop.Value.GetInt32();
                        break;
                    case "tip":
                        isEmri.Tip = (IsEmriTipi)prop.Value.GetInt32();
                        break;
                    case "aracid":
                        isEmri.AracId = prop.Value.ValueKind == JsonValueKind.Null ? null : prop.Value.GetInt32();
                        break;
                    case "durakid":
                        isEmri.DurakId = prop.Value.ValueKind == JsonValueKind.Null ? null : prop.Value.GetInt32();
                        break;
                }
            }

            _context.IsEmri.Update(isEmri);
            await _context.SaveChangesAsync();

            return Ok(isEmri);
        }

    }
}



