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

        public async Task<IActionResult> Index(string razonCambio, int? idProducto, int page = 1)
        {
            int pageSize = 5; // Número de elementos por página

            // Verificar niveles de acceso
            if (!(User.HasClaim("AccessTipe", "Nivel 4") || User.HasClaim("AccessTipe", "Nivel 5")))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            var query = _context.modelHistorialProduct.AsQueryable();

            // Aplicar filtro por RazonCambioAuto si está presente
            if (!string.IsNullOrEmpty(razonCambio))
            {
                query = query.Where(h => h.RazonCambioAuto == razonCambio);
            }

            // Aplicar filtro por IdProduct si está presente
            if (idProducto.HasValue)
            {
                query = query.Where(h => h.IdProduct == idProducto.Value);
            }

            // Contar el total de registros después de los filtros
            int totalRegistros = await query.CountAsync();

            // Aplicar paginación
            var historial = await query
                .OrderByDescending(h => h.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pasar datos a la vista
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.RazonCambio = razonCambio;
            ViewBag.IdProducto = idProducto;

            return View(historial);
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
