using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración del archivo appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el DbContext con la cadena de conexión
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login"; // Ruta al formulario de inicio de sesión
        options.Cookie.HttpOnly = true; // Impide el acceso desde JavaScript (protección XSS)
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Cambiar a 'Always' si usas HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Restringe el envío de la cookie en solicitudes cruzadas
        options.Cookie.Name = "AuthCookie"; // Nombre único para la cookie
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Duración de la cookie
        options.SlidingExpiration = true; // Renueva la cookie antes de expirar si hay actividad
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// Manejo de excepciones en entorno de producción
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Middleware para servir archivos estáticos (CSS, JS, imágenes)
app.UseStaticFiles();

// Configuración de ruteo
app.UseRouting();

// Middleware para autenticación y autorización
app.UseAuthentication(); // Este debe ir antes de UseAuthorization
app.UseAuthorization();

// Configuración de rutas de controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
