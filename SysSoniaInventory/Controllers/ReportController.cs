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
    public class ReportController : Controller
    {
        private readonly DBContext _context;

        public ReportController(DBContext context)
        {
            _context = context;
        }

        // GET: Report
        public async Task<IActionResult> Index()
        {
            return View(await _context.modelReport.ToListAsync());
        }

        // GET: Report/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelReport = await _context.modelReport
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelReport == null)
            {
                return NotFound();
            }

            return View(modelReport);
        }

        // GET: Report/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeReport,Description,Estatus,NameUser,ComentaryUser,StarDate,StarTime,EndDate,EndTime,IdRelation")] ModelReport modelReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelReport);
        }

        // GET: Report/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelReport = await _context.modelReport.FindAsync(id);
            if (modelReport == null)
            {
                return NotFound();
            }
            return View(modelReport);
        }

        // POST: Report/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeReport,Description,Estatus,NameUser,ComentaryUser,StarDate,StarTime,EndDate,EndTime,IdRelation")] ModelReport modelReport)
        {
            if (id != modelReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelReportExists(modelReport.Id))
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
            return View(modelReport);
        }

        // GET: Report/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelReport = await _context.modelReport
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelReport == null)
            {
                return NotFound();
            }

            return View(modelReport);
        }

        // POST: Report/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelReport = await _context.modelReport.FindAsync(id);
            if (modelReport != null)
            {
                _context.modelReport.Remove(modelReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelReportExists(int id)
        {
            return _context.modelReport.Any(e => e.Id == id);
        }
    }
}
