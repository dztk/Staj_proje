using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using Proje.Services;
using System.Threading.Tasks;
//using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args); 

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
    });


builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<ToPdfService>();
builder.Services.AddScoped<ReminderService>();



builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();

var app = builder.Build();



// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    

app.MapRazorPages();


// ARAC EKLEME
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var random = new Random();

    if (!db.Araclar.Any())
    {
        var markaListesi = new Dictionary<string, string>
        {
            { "Akia", "Otobüs" },
            { "Mercedes-Benz", "Otobüs" },
            { "Otokar", "Metrobüs" },
            { "Karsan", "Elektrikli" },
            { "BMC", "Özel Halk Otobüsü" },
            { "Temsa", "Özel Halk Otobüsü" }
        };

        int toplam = 0;

        foreach (var marka in markaListesi)
        {
            string markaAdi = marka.Key;
            string tip = marka.Value;
            char harf = markaAdi[0];

            int adet = tip switch
            {
                "Otobüs" => 28,
                "Metrobüs" => 14,
                "Elektrikli" => 5,
                "Özel Halk Otobüsü" => 30,
                _ => 0
            };

            for (int i = 0; i < adet; i++)
            {
                string plaka = $"34 {RandomHarf()}{RandomHarf()} {1000 + toplam}";
                string kapiNo = $"{harf}-{random.Next(100, 9999)}";

                db.Araclar.Add(new Arac
                {
                    Plaka = plaka,
                    KapiNo = kapiNo,
                    Marka = markaAdi,
                    Model = "ModelX",
                    Tip = tip,
                    KM = random.Next(80000, 250000)
                });

                toplam++;
            }
        }

        db.SaveChanges();
    }

    char RandomHarf() => (char)random.Next('A', 'Z' + 1);
}


// ROL EKLEME
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await CreateRoles(serviceProvider); //  Rol ekleme işlemi  
}

async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Şef", "Teknisyen" };

    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

await app.RunAsync();

