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
        {  // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso
               
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
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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

            if (id == null)
            {
                return NotFound();
            }

            var modelUser = await _context.modelUser.FindAsync(id);
            if (modelUser == null)
            {
                return NotFound();
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
        { "Nivel 4", 4 },
        { "Nivel 3", 3 },
        { "Nivel 2", 2 },
        { "Nivel 1", 1 }
    };

            return nivelesAcceso[rolEditado.AccessTipe] > nivelesAcceso[currentUserAccessTipe];
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser)
        {
            var currentUserAccessTipe = User.FindFirst("AccessTipe")?.Value;
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }

            if (id != modelUser.Id)
            {
                return NotFound();
            }

         

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si la contraseña fue modificada
                    var existingUser = await _context.modelUser.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

                    if (existingUser == null)
                    {
                        return NotFound();
                    }
                    // Verificar que el usuario no esté intentando dar un nivel superior a sí mismo
                    if (EsNivelSuperior(modelUser.IdRol, currentUserAccessTipe)) { 
                        TempData["Error"] = "No puedes asignar un nivel de acceso superior al tuyo.";
                        // Configurar las vistas para seleccionar roles y sucursales en caso de error
                        ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
                        ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
                        return View(); 
                    }


                    if (!string.IsNullOrEmpty(modelUser.Password) && modelUser.Password != existingUser.Password)
                    {
                        // Encriptar la nueva contraseña solo si fue modificada
                  
                        modelUser.Password = SysSoniaInventory.Task.SecurityHelper.EncryptSHA256(modelUser.Password, _secretKey);
                    }
                    else
                    {
                        // Mantener la contraseña actual si no se cambió
                        modelUser.Password = existingUser.Password;
                    }

                    // Actualizar el usuario
                    _context.Update(modelUser);
                    await _context.SaveChangesAsync();
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

            return View(modelUser);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

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
