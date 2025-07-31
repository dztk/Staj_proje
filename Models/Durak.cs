using System.ComponentModel.DataAnnotations;

namespace Proje.Models
{
    public class Durak
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Kod { get; set; } // A-001 gibi

        [Display(Name = "Konum Bilgisi")]
        public Konum KonumBilgi { get; set; }


        public List<IsEmri> IsEmirleri { get; set; } = new();
    }
}
