using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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

            int pageSize = 5; // Número de facturas por página
            var facturasQuery = _context.modelFactura
                .Include(f => f.DetalleFactura)
                .AsQueryable();

            // Filtrar facturas según el nivel de acceso
            if (accessLevel == "Nivel 2" || accessLevel == "Nivel 3")
            {
                facturasQuery = facturasQuery.Where(f => f.NameUser == userName);
            }

            // Aplicar filtros de búsqueda
            if (!string.IsNullOrEmpty(searchUser))
            {
                facturasQuery = facturasQuery.Where(f => f.NameUser.Contains(searchUser));
            }

            if (startDate.HasValue)
            {
                facturasQuery = facturasQuery.Where(f => f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                facturasQuery = facturasQuery.Where(f => f.Date <= endDate.Value);
            }

            // Ordenar y paginar los resultados
            var facturas = await facturasQuery
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Preparar datos para la vista
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)facturasQuery.Count() / pageSize);
            ViewBag.SearchUser = searchUser;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(facturas);
        }


        [HttpPost]
        public IActionResult FiltrarProductos(string nombre, string codigo)
        {
            // Filtrar productos según los parámetros proporcionados
            var productosFiltrados = _context.modelProduct.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                productosFiltrados = productosFiltrados.Where(p => p.Name.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(codigo))
            {
                productosFiltrados = productosFiltrados.Where(p => p.Codigo.Contains(codigo));
            }

            return PartialView("_ProductosFiltrados", productosFiltrados.ToList());
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

            // Filtrar solo productos activos
            ViewBag.Productos = _context.modelProduct.Include(p => p.IdMarcanavigation).Where(p => p.Estatus == 1).ToList();

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
            ModelState.Remove("TotalVenta");
            factura.NameUser = User.Identity?.Name;
            factura.NameSucursal = User.FindFirst("Sucursal")?.Value;
            factura.Date = DateOnly.FromDateTime(DateTime.Now);
            factura.Time = TimeOnly.FromDateTime(DateTime.Now);
            // Validar que los datos son correctos

            ModelState.Remove("PurchasePriceUnitario");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error inesperado en la validación de un campo o más.";
                ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                ViewBag.NameUser = User.Identity?.Name;

                ViewBag.Productos = _context.modelProduct.ToList();
                return View(factura);
            }

            if (detalles == null || !detalles.Any())
            {
                TempData["Error"]= ("Debe agregar al menos un detalle(producto) a la factura.");
                return RedirectToAction("Create", "Factura");
            }

            foreach (var detalle in detalles)
            {
                if (detalle.CantidadProduct <= 0)
                {
                    TempData["Error"] = ("La cantidad de un producto debe ser mayor a cero.");
                    ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                    ViewBag.NameUser = User.Identity?.Name;

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
                        detalle.PurchasePriceUnitario = producto.PurchasePrice;
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
                                    Description = $"Descripción automática: El producto '{producto.Name}' con el código '{producto.Codigo}' e id '{producto.Id}' tiene el stock bajo con {producto.Stock} cantidad a la hora y fecha de creación del reporte, por medio de la sección 'Pago'.",
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
                    TempData["Success"] = "Factura creada correctamente.";
                  
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





        public IActionResult GeneratePdf(DateOnly? startDate, DateOnly? endDate)
        {
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }


            var facturas = _context.modelFactura
                    .Include(f => f.DetalleFactura)
                    .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                facturas = facturas.Where(f => f.Date >= startDate && f.Date <= endDate);
            }

            var facturasList = facturas.ToList();
            if (!facturasList.Any())
            {
                TempData["Error"] = "No se encontraron facturas en el rango de fechas especificado.";
                return RedirectToAction("Index");
            }

            // Calcular el total por factura y la suma total de todas las facturas
            foreach (var factura in facturasList)
            {
                factura.TotalVenta = factura.DetalleFactura.Sum(d => d.PriceTotal);
            }

            decimal totalGeneral = facturasList.Sum(f => (decimal)f.TotalVenta);
            decimal totalVentaBruta = facturasList.Sum(f => f.DetalleFactura.Sum(d => d.PurchasePriceUnitario * d.CantidadProduct));
            decimal gananciaTotal = totalGeneral - totalVentaBruta;

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                // Crear una tabla invisible (sin bordes) con dos columnas: Logo y Título
                var headerTable = new Table(new float[] { 1, 4 }).SetWidth(UnitValue.CreatePercentValue(100));
                headerTable.SetBorder(Border.NO_BORDER); // Quitar bordes de la tabla

                // Celda del logo
                var logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100).SetMarginTop(-50); // Mover 50% más arriba
                logoCell.Add(logo);

                // Celda del título
                var titleCell = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                string title = startDate.HasValue && endDate.HasValue
                    ? $"Facturas del {startDate} al {endDate}"
                    : "Todas las Facturas";
                titleCell.Add(new Paragraph(title)
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetMarginBottom(10));

                // Agregar las celdas a la tabla
                headerTable.AddCell(logoCell);
                headerTable.AddCell(titleCell);

                // Agregar la tabla con logo y título al documento
                document.Add(headerTable);



                // Agregar textos con los totales
                document.Add(new Paragraph($"Total General de Ventas: {totalGeneral:C}")
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginBottom(5));

                document.Add(new Paragraph($"Ganancia Total: {gananciaTotal:C}")
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginBottom(20));

                // Crear tabla con la columna extra para el total de cada factura
                var table = new Table(new float[] { 1, 2, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));

                // Encabezados de la tabla
                var headerColor = new DeviceRgb(41, 128, 185);
                foreach (var header in new[] { "ID", "Sucursal", "Usuario", "Cliente", "Fecha", "Total Factura" })
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetBold())
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(8));
                }

                // Filas alternadas
                var alternateRowColor = new DeviceRgb(230, 240, 255);
                bool isAlternate = false;
                foreach (var factura in facturasList)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(factura.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(factura.NameSucursal))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(factura.NameUser))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(factura.NameClient ?? "N/A"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(factura.Date.ToShortDateString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{factura.TotalVenta:C}")) // Mostrar el total de la factura
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));

                    isAlternate = !isAlternate;
                }

                document.Add(table);

                // Pie de página
                document.Add(new Paragraph("Muebles y Electrodomesticos Sonia")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20));

                document.Close();
                string fechaDescarga = DateTime.Now.ToString("yyyy-MM-dd");
                return File(stream.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}_{fechaDescarga}.pdf");

            }
        }




        [HttpGet]
        public IActionResult DescargarDetallesVentaPdf(int id)
        {
            var venta = _context.modelFactura
                .Include(c => c.DetalleFactura)
                .FirstOrDefault(c => c.Id == id);

            if (venta == null)
            {
                TempData["Error"] = "No se encontró la venta especificada.";
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Agregar logo y título en la misma línea
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                Table headerTable = new Table(new float[] { 1, 3 }).UseAllAvailableWidth();

                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(80, 80);
                    Cell logoCell = new Cell().Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    // Celda vacía si no hay logo
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }


                // Celda del título
                Cell titleCell = new Cell().Add(new Paragraph("Detalles de la venta")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                document.Add(new Paragraph($"Venta ID: {venta.Id}\n" + 
                    $"Sucursal: {venta.NameSucursal}\n" +
                                           $"Usuario: {venta.NameUser}\n" +
                                           $"Fecha: {venta.Date.ToString("dd-MM-yyyy")}\n" +
                                           $"Cliente/Descripción: {venta.NameClient ?? "-"}")
                    .SetFontSize(12)
                    .SetMarginBottom(15));

                var table = new Table(new float[] { 3, 2, 1, 2, 1, 2, 2}).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);

                foreach (var header in new[] { "Producto","Código", "Cant.", "Precio Uni.", "Desc.%", "Precio Desc.", "Total" })
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetBold())
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(8));
                }

                var alternateRowColor = new DeviceRgb(230, 240, 255);
                bool isAlternate = false;
                foreach (var detalle in venta.DetalleFactura)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(detalle.NameProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.SalePriceUnitario:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{(int)detalle.ValorDescuento} %"))
                 .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.SalePriceDescuento:C}"))
                     .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceTotal:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
           
                    isAlternate = !isAlternate;
                }

                document.Add(table);

                document.Add(new Paragraph("Muebles y Electrodomésticos Sonia")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20));

                document.Close();

                string fechaDescarga = DateTime.Now.ToString("yyyy-MM-dd");
                return File(stream.ToArray(), "application/pdf", $"Venta_{venta.Id}_{fechaDescarga}.pdf");
            }
        }

    }
}
