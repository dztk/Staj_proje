using System.ComponentModel.DataAnnotations.Schema;
using Proje.Models;

namespace Proje.Models
{
    [NotMapped]
    public class Konum
    {
        public double Enlem { get; set; }
        public double Boylam { get; set; }
    }
}


   

