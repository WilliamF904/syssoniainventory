using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using System.Security.Claims;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class FacturaController : Controller
    {
        private readonly DBContext _context;

        public FacturaController(DBContext context)
        {
            _context = context;
        }


        // Acción Index para listar facturas
        public IActionResult Index()
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

            IQueryable<ModelFactura> facturasQuery = _context.modelFactura.Include(f => f.DetalleFactura);

            // Filtrar facturas según el nivel de acceso
            if (accessLevel == "Nivel 2" || accessLevel == "Nivel 3")
            {
                facturasQuery = facturasQuery.Where(f => f.NameUser == userName);
            }

            var facturas = facturasQuery.ToList();
            return View(facturas);
        }


        public IActionResult BuscarProducto(string query)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 2 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }

            var productos = _context.modelProduct
                                    .Where(p => p.Estatus == 1 &&
                                                (p.Name.Contains(query) || p.Codigo.ToString().Contains(query)))
                                    .Select(p => new
                                    {
                                        p.Id,
                                        p.Name,
                                        p.Codigo,
                                        p.SalePrice,
                                        p.Stock
                                    })
                                    .ToList();

            return Json(productos);
        }


        // Acción Create para mostrar el formulario de creación
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

        // Acción Create para manejar el envío del formulario
        [HttpPost]
        public IActionResult Create(ModelFactura factura, List<ModelDetalleFactura> detalles)
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

            // Sobrescribir valores de seguridad
            factura.NameUser = User.Identity?.Name;
            factura.NameSucursal = User.FindFirst("Sucursal")?.Value;
            factura.Date = DateOnly.FromDateTime(DateTime.Now);
            factura.Time = TimeOnly.FromDateTime(DateTime.Now);
            // Validar que los datos son correctos
            if (!ModelState.IsValid)
            {
                ViewBag.Productos = _context.modelProduct.ToList();
                return View(factura);
            }

            if (detalles == null || !detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un detalle a la factura.");
                ViewBag.Productos = _context.modelProduct.ToList();
                return View(factura);
            }

            foreach (var detalle in detalles)
            {
                if (detalle.CantidadProduct <= 0)
                {
                    ModelState.AddModelError("", "La cantidad de un producto debe ser mayor a cero.");
                    ViewBag.Productos = _context.modelProduct.ToList();
                    return View(factura);
                }
            }



            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Guardar la factura
                    _context.modelFactura.Add(factura);
                    _context.SaveChanges(); // Aquí se genera el Id de la factura

                    foreach (var detalle in detalles)
                    {
                        // Obtener los datos actuales del producto seleccionado
                        var producto = _context.modelProduct.FirstOrDefault(p => p.Id == detalle.IdProduct);
                        if (producto == null)
                        {
                            ModelState.AddModelError("", $"El producto con ID {detalle.IdProduct} no existe.");
                            ViewBag.Productos = _context.modelProduct.ToList();
                            transaction.Rollback();
                            return View(factura);
                        }

                        // Validar stock
                        if (detalle.CantidadProduct > producto.Stock)
                        {
                            ModelState.AddModelError("",
                                $"La cantidad seleccionada ({detalle.CantidadProduct}) del producto '{producto.Name}' excede el stock actual ({producto.Stock}).");
                            ViewBag.Productos = _context.modelProduct.ToList();
                            transaction.Rollback();
                            return View(factura);
                        }

                        // Restar la cantidad al stock
                        producto.Stock -= detalle.CantidadProduct;

                        // Asignar valores al detalle de factura
                        detalle.IdFactura = factura.Id;
                        detalle.CodigoProducto = producto.Codigo;
                        detalle.NameProducto = producto.Name;
                        detalle.SalePriceUnitario = producto.SalePrice;
                        detalle.SalePriceDescuento = producto.SalePrice - (producto.SalePrice * detalle.ValorDescuento / 100);
                        detalle.PriceTotal = detalle.SalePriceDescuento * detalle.CantidadProduct;

                        // Guardar el detalle
                        _context.modelDetalleFactura.Add(detalle);




                        // Verificar si el producto tiene stock bajo y si se debe crear un reporte
                        if (producto.LowStock >= 0 && producto.Stock <= producto.LowStock)
                        {
                            // Verificar si ya existe un reporte con el IdRelation igual al producto
                            var reportesExistentes = _context.modelReport.Where(r => r.IdRelation == producto.Id).ToList();

                            bool crearReporte = true;

                            foreach (var reporte in reportesExistentes)
                            {
                                if (reporte.Estatus != "Finalizado")
                                {
                                    crearReporte = false;
                                    break;
                                }
                            }

                            if (crearReporte)
                            {
                                // Crear un nuevo reporte si no existe uno o si todos los reportes existentes están finalizados
                                var nuevoReporte = new ModelReport
                                {
                                    NameUser = "",
                                    ComentaryUser = "",
                                    TypeReport = "Stock Bajo",
                                    Description = $"El producto '{producto.Name}' con el código '{producto.Codigo}' e id '{producto.Id}' tiene el stock bajo con {producto.Stock} cantidad a la hora y fecha de creación del reporte.",
                                    Estatus = "Pendiente",
                                    StarDate = DateOnly.FromDateTime(DateTime.Now),
                                    StarTime = TimeOnly.FromDateTime(DateTime.Now),
                                    IdRelation = producto.Id
                                };

                                _context.modelReport.Add(nuevoReporte);
                            }
                        }



                    }

                    // Guardar cambios y confirmar transacción
                    _context.SaveChanges();
                    transaction.Commit();

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
                    return View(factura);
                }
            }
        }




        // Acción para mostrar los detalles de una factura
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
            // Obtener la factura y sus detalles
            var factura = _context.modelFactura
                .Include(f => f.DetalleFactura) // Incluir los detalles de la factura
                .FirstOrDefault(f => f.Id == id);

            if (factura == null)
            {
                TempData["Error"] = "Factura no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar el nivel de acceso
            var accessLevel = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Si es Nivel 2 o Nivel 3, verificar que la factura le corresponda
            if ((accessLevel == "Nivel 2" || accessLevel == "Nivel 3") && factura.NameUser != userName)
            {
                TempData["Error"] = "No tienes permisos para acceder a esta factura.";
                return RedirectToAction(nameof(Index));
            }


            // Pasar los datos a la vista
            return View(factura);
        }


    }
}
