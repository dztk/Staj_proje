using System.ComponentModel.DataAnnotations;

namespace Proje.Models
{
    public class Personel
    {
        public int Id { get; set; }

        [Display(Name = "Adı Soyadı")]
        public string AdSoyad { get; set; }
        public string Unvan { get; set; } // şef-teknisyen

        [Display(Name = "Email")]

        public string KullaniciId { get; set; } //username ile eşleştir


        // Ters yönler:
        public List<IsEmri> IsEmirleri { get; set; } = new();
        public List<BakimHareket> BakimHareketleri { get; set; } = new();
        public List<ParcaIsEmri> ParcaIsEmirleri { get; set; } = new();
    }
}
