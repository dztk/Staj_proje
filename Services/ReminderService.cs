using Proje.Enums;
using Proje.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Proje.Services
{
    public class ReminderService
    {
        private readonly AppDbContext _context;

        public ReminderService(AppDbContext context)
        {
            _context = context;
        }
        public Dictionary<IsEmri, string> GetHatirlatmalar(string rol, int personelId)
        {
            var bugun = DateTime.Now;
            var isEmirleri = _context.IsEmri
                .Include(i => i.Arac)
                .Include(i => i.Durak)
                .ToList();

            var sonuc = new Dictionary<IsEmri, string>();

            foreach (var isEmri in isEmirleri)
            {
                if (isEmri.Durum == IsEmriDurumu.Kapali)
                    continue;

                var gunFarki = (bugun - isEmri.AcilisTarihi).Days;
                string mesaj = null;

                string hedef = isEmri.Tip == IsEmriTipi.Arac
                    ? (isEmri.Arac != null ? isEmri.Arac.Plaka + " plakalı araç" : "Araç")
                    : (isEmri.Durak != null ? isEmri.Durak.Ad + " adlı durak" : "Durak");

                if (rol == "Şef" && isEmri.Durum == IsEmriDurumu.Acik)
                {
                    if (gunFarki >= 15) mesaj = $"{hedef} 15 gündür açık";
                    else if (gunFarki >= 7) mesaj = $"{hedef} 7 gündür açık";
                    else if (gunFarki >= 3) mesaj = $"{hedef} 3 gündür açık";
                }
                else if (rol == "Teknisyen"
                         && (isEmri.Durum == IsEmriDurumu.Bekleme || isEmri.Durum == IsEmriDurumu.Devam)
                         && isEmri.PersonelId == personelId)
                {
                    if (gunFarki >= 15) mesaj = $"{hedef} 15 gündür devam ediyor";
                    else if (gunFarki >= 7) mesaj = $"{hedef} 7 gündür devam ediyor";
                    else if (gunFarki >= 3) mesaj = $"{hedef} 3 gündür devam ediyor";
                }

                if (!string.IsNullOrEmpty(mesaj))
                {
                    sonuc.Add(isEmri, mesaj);
                }
            }

            return sonuc;
        }

    }
}

