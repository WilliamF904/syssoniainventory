using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.Task;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly DBContext _context;
        private readonly string _secretKey;

        public UserController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration["Security:SecretKey"];

            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new Exception("La llave secreta no se encuentra configurada.");
            }
        }
        // GET: User
      
        public async Task<IActionResult> Index()
        {    // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }

            var dBContext = _context.modelUser.Include(m => m.IdRolNavigation).Include(m => m.IdSucursalNavigation);
            return View(await dBContext.ToListAsync());
        }

        // GET: User/Details/5
     
        public async Task<IActionResult> Details(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 2 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 1"))
            {
                // Nivel 1 tiene acceso
               
            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 1 o superior.";
                return RedirectToAction("Index", "Home");
            }




            if (id == null)
            {
                return NotFound();
            }

            var modelUser = await _context.modelUser
                .Include(m => m.IdRolNavigation)
                .Include(m => m.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelUser == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name");
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name");
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }



            if (ModelState.IsValid)
            {
                
                // Encriptar la contraseña antes de guardar usando la llave secreta
                if (!string.IsNullOrEmpty(modelUser.Password))
                {
                    modelUser.Password = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(modelUser.Password, _secretKey);
                }


                // Agregar el usuario a la base de datos
                _context.Add(modelUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Configurar las vistas para seleccionar roles y sucursales en caso de error
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

            return View(modelUser);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;

            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var modelUser = await _context.modelUser.FindAsync(id);
            if (modelUser == null)
            {
                TempData["Error"] = "El usuario a editar no se encontro en la base de datos.";
                return RedirectToAction("Index", "Home");
            }

            if (EsNivelSuperior(modelUser.IdRol, currentUserAccessTipe))
            {
                TempData["Error"] = "No puedes editar a un usuario de nivel superior al tuyo.";
                return RedirectToAction("Index", "Home");
            }
                ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }
        // Método para verificar si el rol del usuario editado es superior al del usuario actual
        private bool EsNivelSuperior(int idRolEditado, string currentUserAccessTipe)
        {
            var rolEditado = _context.modelRol.Find(idRolEditado);
            if (rolEditado == null)
            {
                return false;
            }

            // Definir niveles de acceso
            var nivelesAcceso = new Dictionary<string, int>
    {
                 { "Nivel 5", 5 },
        { "Nivel 4", 4 },
        { "Nivel 3", 3 },
        { "Nivel 2", 2 },
        { "Nivel 1", 1 }
    };

            return nivelesAcceso[rolEditado.AccessTipe] > nivelesAcceso[currentUserAccessTipe];
        }
        private bool EsNivelIgualOSuperior(int idRolEditado, string currentUserAccessTipe)
        {
            // Recuperar el rol que se quiere asignar
            var rolEditado = _context.modelRol.Find(idRolEditado);
            if (rolEditado == null)
            {
                return false; // Retornar falso si no se encuentra el rol
            }

            // Definir niveles de acceso
            var nivelesAcceso = new Dictionary<string, int>
    {
        { "Nivel 5", 5 },
        { "Nivel 4", 4 },
        { "Nivel 3", 3 },
        { "Nivel 2", 2 },
        { "Nivel 1", 1 }
    };

            // Comparar niveles: Retornar true si el nivel es igual o superior al actual
            return nivelesAcceso[rolEditado.AccessTipe] >= nivelesAcceso[currentUserAccessTipe];
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser, string currentPasswordIdentity)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
   

            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }

            if (id != modelUser.Id)
            {
                return NotFound();
            }
            // Verificar si el ID se puede convertir a int (si aplica)
            if (!int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction(nameof(Index));
            }

            var existingUserIdentity = await _context.modelUser.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            var encryptedPasswordIdentity = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(currentPasswordIdentity, _secretKey);
            // Comparar las contraseñas
            if (encryptedPasswordIdentity != existingUserIdentity.Password)
            {
                TempData["Error"] = "Contraseña de autenticación inválida.";

                // Configurar las vistas para roles y sucursales
                ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                return View(modelUser);
            }

         
            var existingUser = await _context.modelUser.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }
         
            modelUser.RegistrationDate = existingUser.RegistrationDate;

            if (ModelState.IsValid)
            {
                try
                {
                  

                    

                    // Verificar que el usuario no esté intentando dar un nivel superior a sí mismo
                    if (EsNivelSuperior(modelUser.IdRol, currentUserAccessTipe)) { 
                        TempData["Error"] = "No puedes asignar un nivel de acceso superior al tuyo.";
                        // Configurar las vistas para seleccionar roles y sucursales en caso de error
                        ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                        ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
                        return View(modelUser); 
                    }

                    if (EsNivelIgualOSuperior(modelUser.IdRol, currentUserAccessTipe))
                    {
                        TempData["Error"] = "No puedes asignar un nivel de acceso igual o superior al tuyo.";

                        // Configurar las vistas para seleccionar roles y sucursales en caso de error
                        ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                        ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                        return View(modelUser); // Retorna la vista con el modelo
                    }

                    if (!string.IsNullOrEmpty(modelUser.Password))
                    {
                        // Encriptar la nueva contraseña solo si fue modificada

                        var encryptedPassword = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(modelUser.Password, _secretKey);

                        if (encryptedPassword != existingUser.Password)
                        {
                            // Si la contraseña encriptada es diferente, actualizarla
                            modelUser.Password = encryptedPassword;
                        }
                        else
                        {
                            // Mantener la contraseña actual si es la misma
                            modelUser.Password = existingUser.Password;
                        }
                    }
                    else
                    {
                        // Mantener la contraseña actual si no se cambió
                        modelUser.Password = existingUser.Password;
                    }

                  

                    _context.Update(modelUser);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Usuario actualizado correctamente.";
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelUserExists(modelUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Configurar las vistas para seleccionar roles y sucursales en caso de error
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelUser);
        }






        // GET: User/Edit/5
        public async Task<IActionResult> EditInfoPerso()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            // Buscar la información completa del usuario autenticado, incluyendo sus relaciones
            var user = await _context.modelUser
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["Error"] = "El usuario no se encontró en la base de datos.";
                return RedirectToAction("Index", "Home");
            }

            // Configurar datos para la vista
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", user.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", user.IdSucursal);

            return View(user);
        }


        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfoPerso([Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser, string currentPasswordIdentity)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            // Buscar al usuario actual en la base de datos
            var existingUserIdentity = await _context.modelUser.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (existingUserIdentity == null)
            {
                TempData["Error"] = "El usuario a editar no se encontró en la base de datos.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar la contraseña actual
            var encryptedPasswordIdentity = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(currentPasswordIdentity, _secretKey);
            if (encryptedPasswordIdentity != existingUserIdentity.Password)
            {
                TempData["Error"] = "Contraseña de autenticación inválida.";

                ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                return View(modelUser);
            }

            // Detectar intentos de modificar campos no permitidos
            var attemptedChanges = new List<string>();

            if (modelUser.IdRol != existingUserIdentity.IdRol) attemptedChanges.Add("Rol");
            if (modelUser.IdSucursal != existingUserIdentity.IdSucursal) attemptedChanges.Add("Sucursal");
            if (modelUser.Name != existingUserIdentity.Name) attemptedChanges.Add("Nombre");
            if (modelUser.LastName != existingUserIdentity.LastName) attemptedChanges.Add("Apellido");
            if (modelUser.RegistrationDate != existingUserIdentity.RegistrationDate) attemptedChanges.Add("Fecha de registro");
            if (modelUser.Estatus != existingUserIdentity.Estatus) attemptedChanges.Add("Estatus");
            if (modelUser.Password != existingUserIdentity.Password) attemptedChanges.Add("Contraseña");


            if (attemptedChanges.Any())
            {
                TempData["Warning"] = $"Algunos campos no pueden ser modificados: {string.Join(", ", attemptedChanges)}.";
            }

            modelUser.IdRol = existingUserIdentity.IdRol;
            modelUser.IdSucursal = existingUserIdentity.IdSucursal;
            modelUser.Name = existingUserIdentity.Name;
            modelUser.LastName = existingUserIdentity.LastName;
            modelUser.RegistrationDate = existingUserIdentity.RegistrationDate;
            modelUser.Estatus = existingUserIdentity.Estatus;
            modelUser.Password = existingUserIdentity.Password;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelUser);
                    await _context.SaveChangesAsync();
                   
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Message"] = "Tu información ha sido actualizada correctamente. Por motivos de seguridad, hemos cerrado tu sesión. Por favor, inicia sesión nuevamente con tus credenciales actualizadas.";
                    if (TempData["Warning"] != null)
                    {
                        TempData["Error"] += $" {TempData["Warning"]}";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelUserExists(modelUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Login", "Auth");
            }

            // Configurar las vistas para seleccionar roles y sucursales en caso de error
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelUser);
        }









        // GET: User/Edit/5
        public async Task<IActionResult> EditUser()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            // Buscar la información completa del usuario autenticado, incluyendo sus relaciones
            var user = await _context.modelUser
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["Error"] = "El usuario no se encontró en la base de datos.";
                return RedirectToAction("Index", "Home");
            }

            // Configurar datos para la vista
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", user.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", user.IdSucursal);

            return View(user);
        }


        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser([Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser, string currentPasswordIdentity)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            // Buscar al usuario actual en la base de datos
            var existingUserIdentity = await _context.modelUser.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (existingUserIdentity == null)
            {
                TempData["Error"] = "El usuario a editar no se encontró en la base de datos.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar la contraseña actual
            var encryptedPasswordIdentity = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(currentPasswordIdentity, _secretKey);
            if (encryptedPasswordIdentity != existingUserIdentity.Password)
            {
                TempData["Error"] = "Contraseña de autenticación inválida.";

                ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                return View(modelUser);
            }

            // Detectar intentos de modificar campos no permitidos
            var attemptedChanges = new List<string>();

            if (modelUser.IdRol != existingUserIdentity.IdRol) attemptedChanges.Add("Rol");
            if (modelUser.IdSucursal != existingUserIdentity.IdSucursal) attemptedChanges.Add("Sucursal");
            if (modelUser.Name != existingUserIdentity.Name) attemptedChanges.Add("Nombre");
            if (modelUser.LastName != existingUserIdentity.LastName) attemptedChanges.Add("Apellido");
            if (modelUser.RegistrationDate != existingUserIdentity.RegistrationDate) attemptedChanges.Add("Fecha de registro");
            if (modelUser.Estatus != existingUserIdentity.Estatus) attemptedChanges.Add("Estatus");
            if (modelUser.Password != existingUserIdentity.Password) attemptedChanges.Add("Contraseña");


            if (attemptedChanges.Any())
            {
                TempData["Warning"] = $"Algunos campos no pueden ser modificados: {string.Join(", ", attemptedChanges)}.";
            }

            modelUser.IdRol = existingUserIdentity.IdRol;
            modelUser.IdSucursal = existingUserIdentity.IdSucursal;
            modelUser.Name = existingUserIdentity.Name;
            modelUser.LastName = existingUserIdentity.LastName;
            modelUser.RegistrationDate = existingUserIdentity.RegistrationDate;
            modelUser.Estatus = existingUserIdentity.Estatus;
            modelUser.Password = existingUserIdentity.Password;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelUser);
                    await _context.SaveChangesAsync();

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Message"] = "Tu información ha sido actualizada correctamente. Por motivos de seguridad, hemos cerrado tu sesión. Por favor, inicia sesión nuevamente con tus credenciales actualizadas.";
                    if (TempData["Warning"] != null)
                    {
                        TempData["Error"] += $" {TempData["Warning"]}";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelUserExists(modelUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Login", "Auth");
            }

            // Configurar las vistas para seleccionar roles y sucursales en caso de error
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelUser);
        }

















        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var modelUser = await _context.modelUser
                .Include(m => m.IdRolNavigation)
                .Include(m => m.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelUser == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            var modelUser = await _context.modelUser.FindAsync(id);
            if (modelUser != null)
            {
                _context.modelUser.Remove(modelUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    




        private bool ModelUserExists(int id)
        {
            return _context.modelUser.Any(e => e.Id == id);
        }
    }
}
