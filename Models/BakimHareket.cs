using Proje.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Proje.Models
{
    public class BakimHareket   
    {
        public int Id { get; set; }

        [Display(Name = "İş Emri")]
        public int IsEmriId { get; set; }

        [Display(Name = "İş Emri")]
        public IsEmri? IsEmri { get; set; }

        [Display(Name = "Yapılan İşlem")]
        public string YapilanIslem { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        [Display(Name = "Personel")]

        public int PersonelId { get; set; }
        public Personel? Personel { get; set; }


    }
}
