using System.ComponentModel.DataAnnotations;

namespace Proje.Enums
{
    public enum IsEmriDurumu
    {
        [Display(Name = "AÇIK")]
        Acik,
        [Display(Name = "BEKLEMEDE")]
        Bekleme,
        [Display(Name = "DEVAM")]
        Devam,
        [Display(Name = "KAPALI")]
        Kapali
    }
}
