using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SysSoniaInventory.DataAccess;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración del archivo appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Obtener la ruta del archivo dbconfig.json desde la variable de entorno
string dbConfigPath = Environment.GetEnvironmentVariable("DB_CONFIG_PATH");
Console.WriteLine($"Ruta del archivo de configuración: {dbConfigPath}");  // Depuración

if (string.IsNullOrEmpty(dbConfigPath))
{
    throw new Exception("La ruta de la configuración de la base de datos no está definida.");
}

// Obtener la cadena de conexión desde dbconfig.json
string dbConfigFullPath = Path.Combine(Directory.GetCurrentDirectory(), dbConfigPath);
if (!File.Exists(dbConfigFullPath))
{
    throw new Exception($"El archivo de configuración de la base de datos no se encontró en la ruta: {dbConfigFullPath}");
}

string dbConfigContent = File.ReadAllText(dbConfigFullPath);

// Parsear el contenido del archivo dbconfig.json para obtener la cadena de conexión
var dbConfig = JsonSerializer.Deserialize<Dictionary<string, string>>(dbConfigContent);
string connectionString = dbConfig["ConnectionString"];  // Asumimos que el archivo dbconfig.json tiene la clave "ConnectionString"

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el DbContext con la cadena de conexión
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(connectionString));  // Usamos la cadena de conexión obtenida desde dbconfig.json

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
    app.UseExceptionHandler("/Home/Error500");
}
app.UseStatusCodePages(async context =>
{
    var statusCode = context.HttpContext.Response.StatusCode;

    if (statusCode == 404) // Error 404
    {
   
        context.HttpContext.Response.Redirect("/Home/Error404"); // Redirigir a una vista personalizada
    }
  
});
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
