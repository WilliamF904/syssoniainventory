using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        private readonly DBContext _context;

        public RolController(DBContext context)
        {
            _context = context;
        }

       
        // GET: Rol/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            var roles = _context.modelRol.ToList();
            return View(roles);
        }


        // GET: Rol/Details/5
        public async Task<IActionResult> Details(int? id)
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

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

            var modelRol = await _context.modelRol
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelRol == null)
            {
                return NotFound();
            }

            return View(modelRol);
        }

        // GET: Rol/Create
        public IActionResult Create()
        { // Verificar niveles de acceso
           if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Rol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AccessTipe")] ModelRol modelRol)
        {
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
            if (ModelState.IsValid)
            {
                _context.Add(modelRol);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Rol creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(modelRol);
        }

        // GET: Rol/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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

            var modelRol = await _context.modelRol.FindAsync(id);
            if (modelRol == null)
            {
                return NotFound();
            }
            return View(modelRol);
        }

        // POST: Rol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AccessTipe")] ModelRol modelRol)
        {
           if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            if (id != modelRol.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelRol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelRolExists(modelRol.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Rol modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelRol);
        }

        // GET: Rol/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

            var modelRol = await _context.modelRol
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelRol == null)
            {
                return NotFound();
            }

            return View(modelRol);
        }

        // POST: Rol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar si el usuario tiene el nivel de acceso requerido
            if (!User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }

            var modelRol = await _context.modelRol.FindAsync(id);

            if (modelRol == null)
            {
                TempData["Error"] = "El rol que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar relaciones activas en ModelUser
            var relatedUsers = await _context.modelUser.AnyAsync(u => u.IdRol == id);
            if (relatedUsers)
            {
                TempData["Error"] = "No se puede eliminar el rol porque tiene usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay relaciones, eliminar el rol
            try
            {
                _context.modelRol.Remove(modelRol);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Rol eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el rol: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }



        private bool ModelRolExists(int id)
        {
            return _context.modelRol.Any(e => e.Id == id);
        }
    }
}
