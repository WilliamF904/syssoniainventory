using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
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
            var facturas = _context.modelFactura.Include(f => f.DetalleFactura).ToList();
            return View(facturas);
        }

        public IActionResult BuscarProducto(string query)
        {
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
            ViewBag.Productos = _context.modelProduct.ToList();
            return View();
        }

        // Acción Create para manejar el envío del formulario
        [HttpPost]
        public IActionResult Create(ModelFactura factura, List<ModelDetalleFactura> detalles)
        {
            // Asignar valores automáticos para la factura
            factura.NameUser = "Usuario Fijo"; // Usuario actual (dinámico en el futuro)
            factura.NameSucursal = "Sucursal Fija"; // Sucursal actual (dinámico en el futuro)
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
                        detalle.SalePriceUnitario = producto.SalePrice;
                        detalle.SalePriceDescuento = producto.SalePrice - (producto.SalePrice * detalle.ValorDescuento / 100);
                        detalle.PriceTotal = detalle.SalePriceDescuento * detalle.CantidadProduct;

                        // Guardar el detalle
                        _context.modelDetalleFactura.Add(detalle);
                    }

                    // Guardar cambios y confirmar transacción
                    _context.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Manejo de errores en caso de fallo
                    transaction.Rollback();
                    ModelState.AddModelError("", "Ocurrió un error al procesar la factura. Inténtelo de nuevo.");
                    ViewBag.Productos = _context.modelProduct.ToList();
                    return View(factura);
                }
            }
        }




        // Acción para mostrar los detalles de una factura
        public IActionResult Details(int id)
        {
            // Obtener la factura y sus detalles
            var factura = _context.modelFactura
                .Include(f => f.DetalleFactura) // Incluir los detalles de la factura
                .FirstOrDefault(f => f.Id == id);

            if (factura == null)
            {
                return NotFound();
            }

            // Pasar los datos a la vista
            return View(factura);
        }


    }
}
