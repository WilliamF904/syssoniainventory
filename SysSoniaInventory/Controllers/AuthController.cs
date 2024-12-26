using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.Task;
using System.Security.Claims;

namespace SysSoniaInventory.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBContext _context;
        private readonly string _secretKey;

        public AuthController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration["Security:SecretKey"];

            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new Exception("La llave secreta no se encuentra configurada.");
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
           
            var encryptedPassword = SecurityHelper.EncryptSHA256(password, _secretKey);

            // Verificar credenciales
            var user = _context.modelUser
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .FirstOrDefault(u => u.Email == email && u.Password == encryptedPassword);

            if (user == null)
            {
                TempData["Message"] = "Usuario o contraseña invalida.";
                return View();
            }

            // Crear claims para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Guardar el ID del usuario   
                new Claim(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.IdRolNavigation.Name),
                new Claim("AccessTipe", user.IdRolNavigation.AccessTipe),
                new Claim("Sucursal", user.IdSucursalNavigation.Name),
                new Claim("SucursalAddress", user.IdSucursalNavigation.Address ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }









        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Método POST para manejar el cambio de contraseña
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Obtener el ID del usuario autenticado desde los claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                // Enviar un mensaje explicando el problema
                TempData["Error"] = "Usuario no autenticado, no es posible modificar o realizar esta acción.";
                return RedirectToAction("Login", "Auth"); // Redirigir a Login si el usuario no está autenticado
            }

            // Obtener el usuario autenticado desde la base de datos
            var user = await _context.modelUser.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                // Cerrar sesión activa
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Enviar un mensaje explicando el problema
                TempData["Error"] = "Hubo un problema al verificar tu autenticación. Por seguridad, tu sesión ha sido cerrada.";

                // Redirigir al inicio de sesión
                return RedirectToAction("Login", "Auth");
            }

            // Validar la contraseña actual
            var encryptedCurrentPassword = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(currentPassword, _secretKey);
            if (user.Password != encryptedCurrentPassword)
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return View();
            }

            // Validar que la nueva contraseña coincida con la confirmación
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "La nueva contraseña y la confirmación no coinciden.";
                return View();
            }

            // Actualizar la contraseña del usuario
            user.Password = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(newPassword, _secretKey);
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "La contraseña se ha cambiado correctamente.";
            return View();
        }









    }
}
