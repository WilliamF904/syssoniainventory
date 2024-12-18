using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly DBContext _context;

        public ProveedorController(DBContext context)
        {
            _context = context;
        }

        // GET: Proveedor
        public async Task<IActionResult> Index()
        {
            return View(await _context.modelProveedor.ToListAsync());
        }

        // GET: Proveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProveedor == null)
            {
                return NotFound();
            }

            return View(modelProveedor);
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Tel,Email")] ModelProveedor modelProveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelProveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelProveedor);
        }

        // GET: Proveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor.FindAsync(id);
            if (modelProveedor == null)
            {
                return NotFound();
            }
            return View(modelProveedor);
        }

        // POST: Proveedor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Tel,Email")] ModelProveedor modelProveedor)
        {
            if (id != modelProveedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelProveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelProveedorExists(modelProveedor.Id))
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
            return View(modelProveedor);
        }

        // GET: Proveedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProveedor == null)
            {
                return NotFound();
            }

            return View(modelProveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelProveedor = await _context.modelProveedor.FindAsync(id);
            if (modelProveedor != null)
            {
                _context.modelProveedor.Remove(modelProveedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelProveedorExists(int id)
        {
            return _context.modelProveedor.Any(e => e.Id == id);
        }
    }
}
