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
    public class DevolucionController : Controller
    {
        private readonly DBContext _context;

        public DevolucionController(DBContext context)
        {
            _context = context;
        }

        // GET: Devolucion
        public async Task<IActionResult> Index(string searchUser, DateOnly? startDate, DateOnly? endDate, int page = 1)
        {

            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4") || User.HasClaim("AccessTipe", "Nivel 3") || User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 4 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }
            var accessLevel = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;

            // Obtener el nombre del usuario autenticado
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            int pageSize = 5; // Número de devoluciones por página
            var devolucionesQuery = _context.modelDevolucion
                .Include(f => f.DetalleDevolucion)
                .AsQueryable();

            // Filtrar devoluciones según el nivel de acceso
            if (accessLevel == "Nivel 2" || accessLevel == "Nivel 3")
            {
                devolucionesQuery = devolucionesQuery.Where(f => f.NameUser == userName);
            }

            // Aplicar filtros de búsqueda
            if (!string.IsNullOrEmpty(searchUser))
            {
                devolucionesQuery = devolucionesQuery.Where(f => f.NameUser.Contains(searchUser));
            }

            if (startDate.HasValue)
            {
                devolucionesQuery = devolucionesQuery.Where(f => f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                devolucionesQuery = devolucionesQuery.Where(f => f.Date <= endDate.Value);
            }

            // Ordenar y paginar los resultados
            var devoluciones = await devolucionesQuery
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Preparar datos para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)devolucionesQuery.Count() / pageSize);
            ViewBag.SearchUser = searchUser;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(devoluciones);
        }

        // GET: Devolucion/Details/5
        public IActionResult Details(int id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4") || User.HasClaim("AccessTipe", "Nivel 3") || User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 4 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }
            // Obtener la devolucion y sus detalles
            var devolucion = _context.modelDevolucion
                .Include(f => f.DetalleDevolucion) // Incluir los detalles de la devolucion
                .FirstOrDefault(f => f.Id == id);

            if (devolucion == null)
            {
                TempData["Error"] = "Devolucion no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar el nivel de acceso
            var accessLevel = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Si es Nivel 2 o Nivel 3, verificar que la factura le corresponda
            if ((accessLevel == "Nivel 2" || accessLevel == "Nivel 3") && devolucion.NameUser != userName)
            {
                TempData["Error"] = "No tienes permisos para acceder a esta devolución.";
                return RedirectToAction(nameof(Index));
            }


            // Pasar los datos a la vista
            return View(devolucion);
        }





        // GET: Devolucion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Devolucion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdFactura,NameSucursal,NameUser,NameClient,Date,Time")] ModelDevolucion modelDevolucion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelDevolucion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modelDevolucion);
        }



        private bool ModelDevolucionExists(int id)
        {
            return _context.modelDevolucion.Any(e => e.Id == id);
        }
    }
}
