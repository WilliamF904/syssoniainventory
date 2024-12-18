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
            var roles = _context.modelRol.ToList();
            return View(roles);
        }


        // GET: Rol/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
        {
            return View();
        }

        // POST: Rol/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AccessTipe")] ModelRol modelRol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelRol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelRol);
        }

        // GET: Rol/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AccessTipe")] ModelRol modelRol)
        {
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
                return RedirectToAction(nameof(Index));
            }
            return View(modelRol);
        }

        // GET: Rol/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
            var modelRol = await _context.modelRol.FindAsync(id);
            if (modelRol != null)
            {
                _context.modelRol.Remove(modelRol);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelRolExists(int id)
        {
            return _context.modelRol.Any(e => e.Id == id);
        }
    }
}
