using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portal_Inmobiliario.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<Portal_Inmobiliario.Services.IAgendaService, Portal_Inmobiliario.Services.AgendaService>();

builder.Services.AddControllersWithViews();

/* 👇👇  HABILITAR SESIÓN (en memoria, suficiente para P2)  👇👇 */
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".portal.s";
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
});
/* ☝️☝️  FIN SESIÓN  ☝️☝️ */

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

await Portal_Inmobiliario.Data.Seed.RunAsync(app.Services);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

/* 👇  IMPORTANTE: usar sesión ANTES de auth/endpoints  */
app.UseSession();
// 👇 FALTA ESTO
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
