using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public async Task<IActionResult> Index(string searchType, string status, DateOnly? startDate, DateOnly? endDate, int page = 1)
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


            int pageSize = 5; // Número de reportes por página
            var query = _context.modelReport.AsQueryable();

            // Aplicar filtros si se proporcionan
            if (!string.IsNullOrEmpty(searchType))
            {
                query = query.Where(r => r.TypeReport.Contains(searchType));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Estatus == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.StarDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.StarDate <= endDate.Value);
            }

            // Ordenar y paginar resultados
            var reports = await query
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Preparar datos para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)query.Count() / pageSize);
            ViewBag.SearchType = searchType;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(reports);

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

            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Validar si la cookie existe y aún no ha expirado
                if (Request.Cookies.TryGetValue(reportCookieName, out string existingCookie))
                {
                    ViewBag.ErrorMessage = "Solo puedes crear un reporte cada 24 horas.";
                    return View(modelReport); // O redirigir a una página diferente si prefieres
                }
            }
           

            ModelState.Remove("NameUser");
            ModelState.Remove("ComentaryUser");
            // Verifica si el usuario está autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                // Obtiene el valor del claim y lo convierte a int
                int userId;
                bool isParsed = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

                // Si el claim se pudo convertir, asigna el valor, de lo contrario deja en 0
                modelReport.IdRelation = isParsed ? userId : 0;
            }
            else
            {
              
            }

            modelReport.NameUser = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Usuario no autenticado";
            modelReport.StarDate = DateOnly.FromDateTime(DateTime.Now);
            modelReport.StarTime = TimeOnly.FromDateTime(DateTime.Now);
            modelReport.ComentaryUser = "";
            modelReport.Description = modelReport.Description ?? string.Empty;

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
            ViewBag.NameUser = User.Identity?.Name;
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
            modelReport.ComentaryUser = modelReport.ComentaryUser ?? string.Empty;

            modelReport.NameUser = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Usuario no autenticado";
            modelReport.EndDate = DateOnly.FromDateTime(DateTime.Now);
            modelReport.EndTime = TimeOnly.FromDateTime(DateTime.Now);

            ModelState.Remove("ComentaryUser");
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


        private bool ModelReportExists(int id)
        {
            return _context.modelReport.Any(e => e.Id == id);
        }
    }
}
