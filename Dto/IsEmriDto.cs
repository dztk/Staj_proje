namespace Proje.Dto
{
    public class IsEmriDto
    {
        public int Id { get; set; }
        public string? Tip { get; set; }
        public string? AracPlaka { get; set; }
        public string? DurakAd { get; set; }
        public string? PersonelAdSoyad { get; set; }
        public string? Aciklama { get; set; }
        public string Durum { get; set; }
        public DateTime AcilisTarihi { get; set; }

    }
}
