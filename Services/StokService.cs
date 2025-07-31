namespace Proje.Services
{
    public class StokService
    {
        public int StokDusur(int mevcutStok, int kullanilanAdet)
        {
            return mevcutStok - kullanilanAdet;
        }
    }
}

