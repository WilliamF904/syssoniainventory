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
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }


            return View(await _context.modelReport.OrderByDescending(r => r.Id).ToListAsync());

        }

        // GET: Report/Details/5
        public async Task<IActionResult> Details(int? id)
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

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

            var modelReport = await _context.modelReport
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelReport == null)
            {
                return NotFound();
            }

            return View(modelReport);
        }

        // GET: Report/Create
        [AllowAnonymous]
        public IActionResult Create()
        { 
            return View();
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeReport,Description,Estatus,NameUser,ComentaryUser,StarDate,StarTime,EndDate,EndTime,IdRelation")] ModelReport modelReport)
        {

            const string reportCookieName = "ReportCreationCooldown";

            // Validar si la cookie existe y aún no ha expirado
            if (Request.Cookies.TryGetValue(reportCookieName, out string existingCookie))
            {
                ViewBag.ErrorMessage = "Solo puedes crear un reporte cada 24 horas.";
                return View(modelReport); // O redirigir a una página diferente si prefieres
            }

            if (ModelState.IsValid)
            {
                _context.Add(modelReport);
                await _context.SaveChangesAsync();
            



                // Crear la cookie con tiempo de expiración de 24 horas
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(24),
                    HttpOnly = true, // La cookie no puede ser accedida por JavaScript
                    SameSite = SameSiteMode.Strict // Para mayor seguridad
                };
                Response.Cookies.Append(reportCookieName, "1", cookieOptions);

                return RedirectToAction(nameof(Index));
            }



            return View(modelReport);
        }

        // GET: Report/Edit/5
        public async Task<IActionResult> Edit(int? id)
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

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
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
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
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

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
        { // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
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
