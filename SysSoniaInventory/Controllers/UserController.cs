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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Layout.Borders;

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

        public async Task<IActionResult> Index(string searchName, string searchLastName, string searchEmail, int? searchEstatus, int? searchIdRol, int page = 1)
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
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener el nivel de acceso del usuario autenticado
            var userAccessTipe = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;

            if (userAccessTipe == null)
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
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

            if (!nivelesAcceso.ContainsKey(userAccessTipe))
            {
                TempData["Error"] = "Nivel de acceso desconocido.";
                return RedirectToAction("Index", "Home");
            }

            int nivelUsuario = nivelesAcceso[userAccessTipe];

            var query = _context.modelUser
                .Include(m => m.IdRolNavigation)
                .Include(m => m.IdSucursalNavigation)
                .AsQueryable();

            // Aplicar filtros si se proporcionan
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(r => r.Name.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchLastName))
            {
                query = query.Where(r => r.LastName.Contains(searchLastName));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(r => r.Email.Contains(searchEmail));
            }
            if (searchEstatus.HasValue)
            {
                query = query.Where(r => r.Estatus == searchEstatus);
            }
            if (searchIdRol.HasValue)
            {
                query = query.Where(r => r.IdRol == searchIdRol);
            }

            // Obtener total de registros antes de paginar
            int totalUsers = await query.CountAsync();

            // Obtener todos los usuarios sin el filtro de niveles
            var users = await query
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * 5)
                .Take(5)
                .ToListAsync();

            // Filtrar los usuarios por niveles de acceso en memoria
            users = users.Where(m => nivelesAcceso[m.IdRolNavigation.AccessTipe] < nivelUsuario ||
                (nivelUsuario == 4 && nivelesAcceso[m.IdRolNavigation.AccessTipe] <= nivelUsuario)).ToList();

            // Obtener el primer usuario para obtener el IdRol, puedes ajustar si necesitas otro criterio
            int? selectedIdRol = users.FirstOrDefault()?.IdRol;

            ViewData["IdRol"] = new SelectList(
                _context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }),
                "Id",
                "Text",
                selectedIdRol // Asignamos el IdRol del primer usuario
            );

            // Datos para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / 5);
            ViewBag.SearchName = searchName;
            ViewBag.SearchLastName = searchLastName;
            ViewBag.SearchEmail = searchEmail;
            ViewBag.SearchEstatus = searchEstatus;
        

            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
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
            // Obtener el nivel de acceso del usuario autenticado
            var userAccessTipe = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;

            if (userAccessTipe == null)
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index");
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

            if (!nivelesAcceso.ContainsKey(userAccessTipe))
            {
                TempData["Error"] = "Nivel de acceso desconocido.";
                return RedirectToAction("Index");
            }

            int nivelUsuario = nivelesAcceso[userAccessTipe];

            if (id == null)
            {
                TempData["Error"] = "Debe seleccionar un usuario.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener el usuario del detalle
            var modelUser = await _context.modelUser
                .Include(m => m.IdRolNavigation)
                .Include(m => m.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modelUser == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si el usuario tiene acceso al detalle
            int nivelDetalle = nivelesAcceso[modelUser.IdRolNavigation.AccessTipe];

            if (nivelDetalle > nivelUsuario || (nivelUsuario != 4 && nivelDetalle == nivelUsuario))
            {
                TempData["Error"] = "No tienes permiso para ver este detalle.";
                return RedirectToAction("Index");
            }

            // Configurar las listas de selección para la vista
            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }), "Id", "Text", modelUser.IdRol);
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
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new {r.Id,Text = r.Name + " - " + r.AccessTipe}),"Id","Text");

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
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }


            ModelState.Remove("RegistrationDate");
            if (ModelState.IsValid)
            {
                
                // Encriptar la contraseña antes de guardar usando la llave secreta
                if (!string.IsNullOrEmpty(modelUser.Password))
                {
                    modelUser.Password = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(modelUser.Password, _secretKey);
                }
                if (EsNivelIgualOSuperior(modelUser.IdRol, currentUserAccessTipe))
                {
                    TempData["Error"] = "No puedes asignar un nivel de acceso igual o superior al tuyo.";

                    // Configurar las vistas para seleccionar roles y sucursales en caso de error
                    ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                    ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                    return View(modelUser); // Retorna la vista con el modelo
                }

                modelUser.RegistrationDate = DateOnly.FromDateTime(DateTime.Now);

                // Agregar el usuario a la base de datos
                _context.Add(modelUser);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado correctamente.";
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
                TempData["Error"] = "Debe seleccionar un usuario.";
                return RedirectToAction(nameof(Index));
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
            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }), "Id", "Text", modelUser.IdRol);
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
                TempData["Error"] = "El id del usuario no coincide.";
                return RedirectToAction(nameof(Index));
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
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
         
            modelUser.RegistrationDate = existingUser.RegistrationDate;
            if (string.IsNullOrEmpty(modelUser.Password))
            {
             
                ModelState.Remove(nameof(modelUser.Password));
            }


            if (ModelState.IsValid)
            {
                try
                {

                    if (userId == id) { 

                    }
                    else if (EsNivelIgualOSuperior(modelUser.IdRol, currentUserAccessTipe))
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
                        TempData["Error"] = "Usuario no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Usuario editado correctamente.";
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
            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }), "Id", "Text", user.IdRol);
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
                        TempData["Error"] = "Usuario no encontrado.";
                        return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> EditUser(int? id)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Verificar si el ID se puede convertir a int (si aplica)
            if (!int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction(nameof(Index));
            }
            if (id == null)
            {
                return NotFound();
            }

            if (id == userId)
            {
                return RedirectToAction("EditInfoPerso", "User");
            }

            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso
                return RedirectToAction("Edit", "User", new { id });

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso y asegura no modificar contraseña

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
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

            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }), "Id", "Text", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }


        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser, string currentPasswordIdentity)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Verificar si el ID se puede convertir a int (si aplica)
            if (!int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction(nameof(Index));
            }



            if (id == userId) {
               return RedirectToAction("EditInfoPerso", "User");
            }


            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso
                return RedirectToAction("Edit", "User", new { id });

            }
            else if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso y asegura no modificar contraseña

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }

            var existingUser = await _context.modelUser.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if (id != modelUser.Id)
            {
               TempData["Error"] = "El id de usuario no coincide.";
                
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


            if (existingUser == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            modelUser.RegistrationDate = existingUser.RegistrationDate;
            if (string.IsNullOrEmpty(modelUser.Password))
            {

                ModelState.Remove(nameof(modelUser.Password));
            }

            if (ModelState.IsValid)
            {
                try
                {



                    if (EsNivelIgualOSuperior(modelUser.IdRol, currentUserAccessTipe))
                    {
                        TempData["Error"] = "No puedes asignar un nivel de acceso igual o superior al tuyo.";

                        // Configurar las vistas para seleccionar roles y sucursales en caso de error
                        ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                        ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);

                        return View(modelUser); // Retorna la vista con el modelo
                    }

                    if (!string.IsNullOrEmpty(modelUser.Password) && User.HasClaim("AccessTipe", "Nivel 4"))
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
                        TempData["Error"] = "Usuario no encontrado.";
                        return RedirectToAction(nameof(Index));
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










        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Verificar si el ID se puede convertir a int (si aplica)
            if (!int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction(nameof(Index));
            }
            if (id == userId)
            {
                TempData["Error"] = "¡No puedes elimininarte a ti mismo!";
                return RedirectToAction("Index", "User");
            }

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
                TempData["Error"] = "Debe seleccionar un usuario.";
                return RedirectToAction(nameof(Index));
            }

            var modelUser = await _context.modelUser
                .Include(m => m.IdRolNavigation)
                .Include(m => m.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelUser == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.modelRol.Select(r => new { r.Id, Text = r.Name + " - " + r.AccessTipe }), "Id", "Text", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string currentPasswordIdentity)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar si el ID se puede convertir a int (si aplica)
            if (!int.TryParse(currentUserId, out int userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction(nameof(Index));
            }
            if (id == userId)
            {
                ViewBag.ErrorMessage = "¡No puedes elimininarte a ti mismo!";
                return RedirectToAction("Index", "User");
            }


            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

 
            var modelUser = await _context.modelUser.FindAsync(id);
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









         
            if (modelUser != null)
            {
                _context.modelUser.Remove(modelUser);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }




        // Método para generar PDF
        public IActionResult GeneratePdf(bool? active = null)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener datos de usuarios (filtrar por estado si se especifica)
            var users = _context.modelUser.AsQueryable();
            if (active.HasValue)
            {
                users = users.Where(u => u.Estatus == (active.Value ? 1 : 0));
            }

            var userList = users.ToList();

            // Generar PDF
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Agregar logo y título en la misma línea
                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                Table headerTable = new Table(new float[] { 1, 3 }).UseAllAvailableWidth();

                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(80, 80);
                    Cell logoCell = new Cell().Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    // Celda vacía si no hay logo
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }

                string title = active.HasValue ? (active.Value ? "Usuarios Activos" : "Usuarios Inactivos") : "Todos los Usuarios";
                // Celda del título
                Cell titleCell = new Cell().Add(new Paragraph(title)
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                // Crear tabla
                var table = new Table(new float[] { 1, 2, 2, 3, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);



                // Encabezados estilizados
                var headerColor = new DeviceRgb(52, 152, 219); // Azul intenso
                foreach (var header in new[] { "ID", "Nombre", "Apellido", "Email", "Estado" })
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetBold())
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(8));
                }

                // Filas alternadas
                var alternateRowColor = new DeviceRgb(235, 245, 255); // Azul claro
                bool isAlternate = false;
                foreach (var user in userList)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(user.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(user.Name))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(user.LastName))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(user.Email))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(user.Estatus == 1 ? "Activo" : "Inactivo"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    isAlternate = !isAlternate;
                }

                document.Add(table);

                // Pie de página
                document.Add(new Paragraph("Muebles y Electrodomésticos Sonia")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20));

                document.Close();
                string fechaDescarga = DateTime.Now.ToString("yyyy-MM-dd");
                // Retornar archivo PDF
                return File(stream.ToArray(), "application/pdf", $"Usuarios_{fechaDescarga}.pdf");
            }
        }


        private bool ModelUserExists(int id)
        {
            return _context.modelUser.Any(e => e.Id == id);
        }
    }
}
