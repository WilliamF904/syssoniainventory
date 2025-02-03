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
using SysSoniaInventory.ViewModels;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly DBContext _context;

        public CategoryController(DBContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {      // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }

            return View(await _context.modelCategory.OrderByDescending(r => r.Id).ToListAsync());
        }

        // GET: Category/Details/5
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
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

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

            var modelCategory = await _context.modelCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelCategory == null)
            {
                return NotFound();
            }

            return View(modelCategory);
        }

        public async Task<IActionResult> ProductosPorCategoria(int id, int page = 1)
        {
            // Verificar si la categoría existe
            var categoria = await _context.modelCategory.FindAsync(id);
            if (categoria == null)
            {
                TempData["Error"] = "Debe seleccionar un id de categoria valido.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de productos por página

            // Obtener la consulta de productos con las relaciones necesarias
            var query = _context.modelProduct
                .Include(p => p.IdCategoryNavigation)
                .Where(p => p.IdCategory == id) // Filtrar por IdCategory
                .AsQueryable();

            // Contar el total de productos para la categoría
            int totalProductos = await query.CountAsync();

            // Aplicar paginación
            var productos = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            // Verificar si no se encontraron productos
            if (!productos.Any())
            {
                TempData["reporte"] = $"No se encontraron productos relacionados con la categoría '{categoria.Name}'.";
            }

            // Pasar datos a la vista
            ViewBag.CategoriaId = id;
            ViewBag.CategoriaNombre = categoria.Name; // Pasar el nombre de la categoría
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProductos / pageSize); // Calcular el total de páginas

            return View(productos);
        }

        // GET: Category/Create
        public IActionResult Create()
        { // Verificar niveles de acceso
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
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ModelCategory modelCategory)
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
            if (ModelState.IsValid)
            {
                _context.Add(modelCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelCategory);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var modelCategory = await _context.modelCategory.FindAsync(id);
            if (modelCategory == null)
            {
                return NotFound();
            }
            return View(modelCategory);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ModelCategory modelCategory)
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
            if (id != modelCategory.Id)
            {
                TempData["Error"] = "Debese seleccionar una categoria.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelCategory);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoria modificada correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelCategoryExists(modelCategory.Id))
                    {
                        TempData["Error"] = "Categoria no encontrada.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelCategory);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var modelCategory = await _context.modelCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelCategory == null)
            {
                return NotFound();
            }

            return View(modelCategory);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") || !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o superior.";
                return RedirectToAction("Index", "Home");
            }


            // Verificar si hay productos asociados a este proveedor
            var relatedProducts = await _context.modelProduct.AnyAsync(p => p.IdCategory == id);
            if (relatedProducts)
            {
                TempData["Error"] = "No se puede eliminar la categoria porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            var modelCategory = await _context.modelCategory.FindAsync(id);
            if (modelCategory != null)
            {
                _context.modelCategory.Remove(modelCategory);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Categoria eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool ModelCategoryExists(int id)
        {
            return _context.modelCategory.Any(e => e.Id == id);
        }
    }
}
