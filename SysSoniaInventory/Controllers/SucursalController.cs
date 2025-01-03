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
    public class SucursalController : Controller
    {
        private readonly DBContext _context;

        public SucursalController(DBContext context)
        {
            _context = context;
        }


        // GET: Sucursal
        public async Task<IActionResult> Index()
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
            return View(await _context.modelSucursal.ToListAsync());
        }

        // GET: Sucursal/Details/5
        public async Task<IActionResult> Details(int? id)
        {// Verificar niveles de acceso
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

            var modelSucursal = await _context.modelSucursal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelSucursal == null)
            {
                return NotFound();
            }

            return View(modelSucursal);
        }

        // GET: Sucursal/Create
        public IActionResult Create()
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
            return View();
        }

        // POST: Sucursal/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] ModelSucursal modelSucursal)
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
            if (ModelState.IsValid)
            {
                _context.Add(modelSucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sucursal creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelSucursal);
        }

        // GET: Sucursal/Edit/5
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

            var modelSucursal = await _context.modelSucursal.FindAsync(id);
            if (modelSucursal == null)
            {
                return NotFound();
            }
            return View(modelSucursal);
        }

        // POST: Sucursal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] ModelSucursal modelSucursal)
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
            if (id != modelSucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelSucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelSucursalExists(modelSucursal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Sucursal modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelSucursal);
        }

        // GET: Sucursal/Delete/5
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

            var modelSucursal = await _context.modelSucursal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelSucursal == null)
            {
                return NotFound();
            }

            return View(modelSucursal);
        }

        // POST: Sucursal/Delete/5
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

            var modelSucursal = await _context.modelSucursal.FindAsync(id);

            if (modelSucursal == null)
            {
                TempData["Error"] = "La sucursal que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si hay usuarios asociados a esta sucursal
            var relatedUsers = await _context.modelUser.AnyAsync(u => u.IdSucursal == id);
            if (relatedUsers)
            {
                TempData["Error"] = "No se puede eliminar la sucursal porque tiene usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay relaciones, eliminar la sucursal
            try
            {
                _context.modelSucursal.Remove(modelSucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sucursal eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar la sucursal: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ModelSucursalExists(int id)
        {
            return _context.modelSucursal.Any(e => e.Id == id);
        }
    }
}
