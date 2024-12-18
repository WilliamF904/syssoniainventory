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
    public class UserController : Controller
    {
        private readonly DBContext _context;

        public UserController(DBContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.modelUser.Include(m => m.IdRolNavigation).Include(m => m.IdSucursalNavigation);
            return View(await dBContext.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name");
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelUser = await _context.modelUser.FindAsync(id);
            if (modelUser == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRol,IdSucursal,Tel,Name,LastName,Email,Password,Estatus,RegistrationDate")] ModelUser modelUser)
        {
            if (id != modelUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["IdRol"] = new SelectList(_context.modelRol, "Id", "Name", modelUser.IdRol);
            ViewData["IdSucursal"] = new SelectList(_context.modelSucursal, "Id", "Name", modelUser.IdSucursal);
            return View(modelUser);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

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
