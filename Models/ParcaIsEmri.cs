using Proje.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proje.Models
{
    public class ParcaIsEmri
    {
        public int Id { get; set; }

        [Display(Name = "Parça Adı")]

        [Required(ErrorMessage = "Parça seçilmelidir.")]
        public int ParcaId { get; set; }
        
        [Display(Name = "Parça Adı")]
        public Parca? Parca { get; set; }  // Navigation

        [Display(Name = "İş Emri")]
        [Required(ErrorMessage = "İş Emri seçilmelidir.")]
        public int IsEmriId { get; set; }

        [Display(Name = "İş Emri")]
        public IsEmri? IsEmri { get; set; }  // Navigation

        [Required(ErrorMessage = "Miktar girilmelidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Miktar { get; set; }

        [Display(Name = "İşçilik Süresi (saat)")]
        public int? IscilikSuresiSaat { get; set; }

        [NotMapped]
        public decimal BirimFiyat => Parca?.BirimFiyat ?? 0;

        [NotMapped]
        public decimal ToplamTutar => Miktar * (Parca?.BirimFiyat ?? 0);

        public int? PersonelId { get; set; } 
        public Personel? Personel { get; set; } 


    }
}