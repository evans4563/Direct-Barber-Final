using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;
using Direct_Barber.Servicios.Contrato;
using Direct_Barber.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DirectBarber1Context>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL"));
});

//Permitir utilizar el servicio desde cualquier controlador.
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

//Agregar la autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/IniciarSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Habilitar la memoria distribuida y la sesión.
builder.Services.AddDistributedMemoryCache();  // Necesario para usar sesiones.
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Configura el tiempo de expiración de la sesión.
    options.Cookie.HttpOnly = true;  // Seguridad.
    options.Cookie.IsEssential = true; // Necesario para el cumplimiento de la GDPR.
});

//Configuracion para deshabilitar el cache
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None,
        });
});

// Agregar soporte para HttpContext en cualquier controlador.
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); //Habilitar la sesión.
app.UseAuthentication(); // Habilitar autenticación.
app.UseAuthorization(); // Habilitar autorización.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=IniciarSesion}/{id?}");

app.Run();