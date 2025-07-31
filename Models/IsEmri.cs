using Proje.Models;
using System;
using Proje.Enums;
using System.ComponentModel.DataAnnotations;



namespace Proje.Models
{
    public class IsEmri
    {
        public int Id { get; set; }
        public IsEmriTipi Tip { get; set; } // enum Arac ya da Durak

        [Display(Name = "Araç")]
        public int? AracId { get; set; } // nullable

        [Display(Name = "Araç")]
        public Arac? Arac { get; set; } // Navigation property

        [Display(Name = "Durak")]

        public int? DurakId { get; set; } // nullable 
        public Durak? Durak { get; set; } // Navigation property

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; } // Cam kırıldı

        public IsEmriDurumu Durum { get; set; } = IsEmriDurumu.Acik; // Açık,Beklemede,Kapalı default açık

        [Display(Name= "Açılış Tarihi")]
        public DateTime AcilisTarihi { get; set; } = DateTime.Now; //şimdiyi al

        [Display(Name = "Kapanış Tarihi")]
        public DateTime? KapanisTarihi { get; set; } // nullable
       
        [Display(Name = "Personel")]
        public int? PersonelId { get; set; } // nullable
        public Personel? Personel { get; set; } // Navigation property

        public string? ImzaPdf { get; set; } //imzalı pdf için alan 


        public List<BakimHareket> BakimHareketleri { get; set; } = new();
        public List<ParcaIsEmri> ParcaIsEmirleri { get; set; } = new();



        // araç durak seçilmezse iş emri oluşmasın

        [CustomValidation(typeof(IsEmri), nameof(ValidateAracOrDurak))]
        public object? DummyValidationTrigger => null;
        public static ValidationResult? ValidateAracOrDurak(object? _, ValidationContext context)
        {
            var model = (IsEmri)context.ObjectInstance;

            if (model.Tip == IsEmriTipi.Arac && model.AracId == null)
            {
                return new ValidationResult("Araç seçilmeden iş emri oluşturulamaz.");
            }

            if (model.Tip == IsEmriTipi.Durak && model.DurakId == null)
            {
                return new ValidationResult("Durak seçilmeden iş emri oluşturulamaz.");
            }

            return ValidationResult.Success;
        }
    }
}

