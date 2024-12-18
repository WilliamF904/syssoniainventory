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
            return View(await _context.modelHistorialProduct.ToListAsync());
        }

        // GET: HistorialProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: HistorialProduct/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HistorialProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameUser,IdProduct,BeforeNameProduct,AfterNameProduct,BeforePurchasePrice,AfterPurchasePrice,BeforeSalePrice,AfterSalePrice,BeforeStock,AfterStock,BeforeCodigo,AfterCodigo,Date,Time,RazonCambioAuto,DescriptionCambio")] ModelHistorialProduct modelHistorialProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelHistorialProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelHistorialProduct);
        }

        // GET: HistorialProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelHistorialProduct = await _context.modelHistorialProduct.FindAsync(id);
            if (modelHistorialProduct == null)
            {
                return NotFound();
            }
            return View(modelHistorialProduct);
        }

        // POST: HistorialProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameUser,IdProduct,BeforeNameProduct,AfterNameProduct,BeforePurchasePrice,AfterPurchasePrice,BeforeSalePrice,AfterSalePrice,BeforeStock,AfterStock,BeforeCodigo,AfterCodigo,Date,Time,RazonCambioAuto,DescriptionCambio")] ModelHistorialProduct modelHistorialProduct)
        {
            if (id != modelHistorialProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelHistorialProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelHistorialProductExists(modelHistorialProduct.Id))
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
            return View(modelHistorialProduct);
        }

        // GET: HistorialProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
