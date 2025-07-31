using System.ComponentModel.DataAnnotations;

namespace Proje.Models
{
    public class Parca
    {   public int Id { get; set; }
        public string Ad { get; set; }

        [Display(Name = "Birim Fiyatı")]
        public decimal BirimFiyat { get; set; }

        [Display(Name = "Stok Miktarı")]
        public int Stok { get; set; }

        public List<ParcaIsEmri> ParcaIsEmirleri { get; set; } = new();
    }
}
