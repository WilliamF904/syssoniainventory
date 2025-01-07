using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuraci�n del archivo appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el DbContext con la cadena de conexi�n
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login"; // Ruta al formulario de inicio de sesi�n
        options.Cookie.HttpOnly = true; // Impide el acceso desde JavaScript (protecci�n XSS)
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Cambiar a 'Always' si usas HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Restringe el env�o de la cookie en solicitudes cruzadas
        options.Cookie.Name = "AuthCookie"; // Nombre �nico para la cookie
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Duraci�n de la cookie
        options.SlidingExpiration = true; // Renueva la cookie antes de expirar si hay actividad
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// Manejo de excepciones en entorno de producci�n
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Middleware para servir archivos est�ticos (CSS, JS, im�genes)
app.UseStaticFiles();

// Configuraci�n de ruteo
app.UseRouting();

// Middleware para autenticaci�n y autorizaci�n
app.UseAuthentication(); // Este debe ir antes de UseAuthorization
app.UseAuthorization();

// Configuraci�n de rutas de controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicaci�n
app.Run();
