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
    public class SucursalController : Controller
    {
        private readonly DBContext _context;

        public SucursalController(DBContext context)
        {
            _context = context;
        }

        // GET: Sucursal
        public async Task<IActionResult> Index()
        {
            return View(await _context.modelSucursal.ToListAsync());
        }

        // GET: Sucursal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            return View();
        }

        // POST: Sucursal/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] ModelSucursal modelSucursal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelSucursal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelSucursal);
        }

        // GET: Sucursal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
                return RedirectToAction(nameof(Index));
            }
            return View(modelSucursal);
        }

        // GET: Sucursal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
            var modelSucursal = await _context.modelSucursal.FindAsync(id);
            if (modelSucursal != null)
            {
                _context.modelSucursal.Remove(modelSucursal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelSucursalExists(int id)
        {
            return _context.modelSucursal.Any(e => e.Id == id);
        }
    }
}
