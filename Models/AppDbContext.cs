using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using System;

namespace Proje.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Arac>? Araclar { get; set; } //nullable
        public DbSet<Durak>? Durak { get; set; }
        public DbSet<Personel> Personel { get; set; }
        public DbSet<Parca> Parca { get; set; }
        public DbSet<IsEmri> IsEmri { get; set; }
        public DbSet<ParcaIsEmri> ParcaIsEmri { get; set; }
        public DbSet<BakimHareket> BakimHareket { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Durak>().OwnsOne(d => d.KonumBilgi);


        }
    }
}

   

