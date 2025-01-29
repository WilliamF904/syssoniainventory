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
        [HttpGet]
        public IActionResult Create()
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


            ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
            ViewBag.NameUser = User.Identity?.Name;

            ViewBag.Productos = _context.modelProduct.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(ModelDevolucion devolucion, List<ModelDetalleDevolucion> detalles)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4") || User.HasClaim("AccessTipe", "Nivel 3") || User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 4 tiene acceso
            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }

            // Sobrescribir valores de seguridad
            devolucion.NameUser = User.Identity?.Name;
            devolucion.NameSucursal = User.FindFirst("Sucursal")?.Value;
            devolucion.Date = DateOnly.FromDateTime(DateTime.Now);
            devolucion.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Validar que los datos son correctos
            ModelState.Remove("PurchasePriceUnitario");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error inesperado en la validación de un campo o más.";
                ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                ViewBag.NameUser = User.Identity?.Name;

                ViewBag.Productos = _context.modelProduct.ToList();
                return View(devolucion);
            }

            if (detalles == null || !detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un detalle a la devolucion.");
                ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                ViewBag.NameUser = User.Identity?.Name;

                ViewBag.Productos = _context.modelProduct.ToList();
                return View(devolucion);
            }

            foreach (var detalle in detalles)
            {
                if (detalle.CantidadProduct <= 0)
                {
                    ModelState.AddModelError("", "La cantidad de un producto debe ser mayor a cero.");
                    ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                    ViewBag.NameUser = User.Identity?.Name;

                    ViewBag.Productos = _context.modelProduct.ToList();
                    return View(devolucion);
                }
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Guardar la devolucion
                    _context.modelDevolucion.Add(devolucion);
                    _context.SaveChanges(); // Aquí se genera el Id de la devolucion

                    foreach (var detalle in detalles)
                    {
                        // Obtener los datos actuales del producto seleccionado
                        var producto = _context.modelProduct.FirstOrDefault(p => p.Id == detalle.IdProduct);
                        if (producto == null)
                        {
                            ModelState.AddModelError("", $"El producto con ID {detalle.IdProduct} no existe.");
                            ViewBag.Productos = _context.modelProduct.ToList();
                            transaction.Rollback();
                            return View(devolucion);
                        }

                        //--------------------------------------------------------------------------------------
                        if (detalle.StockD == "1")
                        {
                            // Almacenar valores originales para el historial
                            var stockAnterior = producto.Stock;

                            // Actualizar stock sumando la cantidad de productos devueltos
                            producto.Stock += detalle.CantidadProduct;

                            // Guardar el historial del cambio de stock
                            var historial = new ModelHistorialProduct
                            {
                                NameUser = User.Identity?.Name,
                                IdProduct = producto.Id,
                                Date = DateOnly.FromDateTime(DateTime.Now),
                                Time = TimeOnly.FromDateTime(DateTime.Now),
                                BeforeStock = stockAnterior,
                                AfterStock = producto.Stock,
                                RazonCambioAuto = "Devolución de productos (Devolucion)",
                                DescriptionCambio = "Devolución registrado y stock actualizado"
                            };

                            _context.Add(historial);

                            // Validar si el stock supera LowStock y hay reportes pendientes
                            if (producto.Stock > producto.LowStock)
                            {
                                var reportesPendientes = _context.modelReport
                                    .Where(r => r.IdRelation == producto.Id && r.TypeReport == "Stock Bajo" && r.Estatus != "Finalizado")
                                    .ToList();

                                foreach (var reporte in reportesPendientes)
                                {
                                    reporte.Estatus = "Finalizado";
                                    reporte.NameUser = User.Identity?.Name;
                                    reporte.ComentaryUser = $"Descripción automática: Se agregó {detalle.CantidadProduct} al stock del producto por medio de la sección 'Devolucion' con el id {detalle.IdDevolucion}.";
                                    reporte.EndDate = DateOnly.FromDateTime(DateTime.Now);
                                    reporte.EndTime = TimeOnly.FromDateTime(DateTime.Now);
                                 
                                    _context.Update(reporte);
                                }
                            }
                        }

                        //--------------------------------------------------------------------------------------


                        // Asignar valores al detalle de compra
                        detalle.IdDevolucion = devolucion.Id;
                        detalle.CodigoProducto = producto.Codigo;
                        detalle.NameProduct = producto.Name;
                        detalle.PriceReembolso = producto.PurchasePrice; // Verificar primero el precio manual
                        detalle.PriceTotalReembolso = detalle.PriceReembolso * detalle.CantidadProduct;

                        // Guardar el detalle
                        _context.modelDetalleDevolucion.Add(detalle);
                    }

                    // Guardar cambios y confirmar transacción
                    _context.SaveChanges();
                    transaction.Commit();
                    TempData["Success"] = "Devolución registrada correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    ModelState.AddModelError("", "Ocurrió un error al guardar los cambios en la base de datos.");
                    ViewBag.Productos = _context.modelProduct.ToList();
                    return View(devolucion);
                }
            }
        }








        private bool ModelDevolucionExists(int id)
        {
            return _context.modelDevolucion.Any(e => e.Id == id);
        }
    }
}
