using System.ComponentModel.DataAnnotations;

namespace Proje.Models
{
    public class Arac
    {
        public int Id { get; set; }
        public string Plaka { get; set; }

        [Display(Name = "Kapı No")]
        public string KapiNo { get; set; } // A-001 gibi
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Tip { get; set; } //  Otobüs=56 Metrobüs=	14 Elektrikli Araç= 5	 Özel Halk Otobüsü= 60	
        public int KM { get; set; }


        public List<IsEmri> IsEmirleri { get; set; } = new();
    }
}
