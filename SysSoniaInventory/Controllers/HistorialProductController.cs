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
    public class HistorialProductController : Controller
    {
        private readonly DBContext _context;

        public HistorialProductController(DBContext context)
        {
            _context = context;
        }

        // GET: HistorialProduct
        public async Task<IActionResult> Index()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
          
            return View(await _context.modelHistorialProduct.OrderByDescending(r => r.Id).ToListAsync());
        }

        // GET: HistorialProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

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

            var modelHistorialProduct = await _context.modelHistorialProduct
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelHistorialProduct == null)
            {
                return NotFound();
            }

            return View(modelHistorialProduct);
        }


        // GET: HistorialProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
         
           if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

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

            var modelHistorialProduct = await _context.modelHistorialProduct
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelHistorialProduct == null)
            {
                return NotFound();
            }

            return View(modelHistorialProduct);
        }

        // POST: HistorialProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 4 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            var modelHistorialProduct = await _context.modelHistorialProduct.FindAsync(id);
            if (modelHistorialProduct != null)
            {
                _context.modelHistorialProduct.Remove(modelHistorialProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ModelHistorialProductExists(int id)
        {
            return _context.modelHistorialProduct.Any(e => e.Id == id);
        }
    }
}
