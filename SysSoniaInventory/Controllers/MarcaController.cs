using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    public class MarcaController : Controller
    {
        private readonly DBContext _context;
       

        public MarcaController(DBContext context)
        {
            _context = context;
        }

        // GET: Marca
        public async Task<IActionResult> Index()
        {
            return View(await _context.modelMarca.ToListAsync());
        }

        // GET: Marca/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelMarca == null)
            {
                return NotFound();
            }

            return View(modelMarca);
        }
        public async Task<IActionResult> ProductosPorMarca(int id, int page = 1)
        {
            // Verificar si la marca existe
            var marca = await _context.modelMarca.FindAsync(id);
            if (marca == null)
            {
                TempData["Error"] = "Debe seleccionar un id de marca valido.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de productos por página

            // Obtener la consulta de productos con las relaciones necesarias
            var query = _context.modelProduct
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Where(p => p.IdMarca == id) // Filtrar por IdMarca
                .AsQueryable();

            // Contar el total de productos para la marca
            int totalProductos = await query.CountAsync();

            // Aplicar paginación
            var productos = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (!productos.Any())
            {
                TempData["reporte"] = $"No se encontraron productos relacionados con la marca '{marca.Name}'.";
            }
            // Pasar datos a la vista
            ViewBag.MarcaId = id;
            ViewBag.MarcaNombre = marca.Name; // Pasar el nombre de la marca
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProductos / pageSize); // Calcular el total de páginas

            return View(productos);
        }

        // GET: Marca/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marca/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ModelMarca modelMarca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelMarca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelMarca);
        }

        // GET: Marca/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca.FindAsync(id);
            if (modelMarca == null)
            {
                return NotFound();
            }
            return View(modelMarca);
        }

        // POST: Marca/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ModelMarca modelMarca)
        {
            if (id != modelMarca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelMarca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelMarcaExists(modelMarca.Id))
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
            return View(modelMarca);
        }

        // GET: Marca/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelMarca == null)
            {
                return NotFound();
            }

            return View(modelMarca);
        }

        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelMarca = await _context.modelMarca.FindAsync(id);
            if (modelMarca != null)
            {
                _context.modelMarca.Remove(modelMarca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelMarcaExists(int id)
        {
            return _context.modelMarca.Any(e => e.Id == id);
        }
    }
}
