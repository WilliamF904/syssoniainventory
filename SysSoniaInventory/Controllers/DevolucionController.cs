using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)devolucionesQuery.Count() / pageSize);
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
                { // Nivel 4 tiene acceso

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
                ModelState.Remove("NameClient");
            ModelState.Remove("IdFactura"); 
                  ModelState.Remove("NameProduct");
            if (devolucion.NameClient == null)
                {
                devolucion.NameClient = "";
                }

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Error inesperado en la validación de un campo o más.";
                return RedirectToAction("Create", "Devolucion");
            }

                if (detalles == null || !detalles.Any())
                {
                    TempData["Error"] = "Debe agregar al menos un detalle a la devolucion.";
                return RedirectToAction("Create", "Devolucion");
            }

                foreach (var detalle in detalles)
                {
                    if (detalle.CantidadProduct <= 0)
                    {
                        TempData["Error"] = "La cantidad de un producto debe ser mayor a cero.";
                    return RedirectToAction("Create", "Devolucion");
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
                                TempData["Error"] = $"El producto con ID {detalle.IdProduct} no existe.";
                            
                                transaction.Rollback();
                            return RedirectToAction("Create", "Devolucion");
                        }
                            // Verifica si se debe actualizar el precio
                            var beforePurchasePrice = producto.PurchasePrice;
                          

                            // Almacenar valores originales para el historial
                            var stockAnterior = producto.Stock;
                        if (detalle.StockD)
                        {
                            // Actualizar stock sumando la cantidad de productos devueltos
                            producto.Stock += detalle.CantidadProduct;
                        }


                        // Asignar valores al detalle de devolucion
                        detalle.IdDevolucion = devolucion.Id;
                            detalle.CodigoProducto = producto.Codigo;
                            detalle.NameProduct = producto.Name;
                            detalle.PriceReembolso = detalle.PriceReembolso; // Usar el precio ingresado por el usuario                                                                
                            detalle.PriceTotalReembolso = detalle.PriceReembolso * detalle.CantidadProduct;
                      
                            // Guardar el detalle
                            _context.modelDetalleDevolucion.Add(detalle);

                        // Guardar el historial del cambio de stock
                        if (detalle.StockD)
                        {
                            var historial = new ModelHistorialProduct
                            {
                                NameUser = User.Identity?.Name,
                                IdProduct = producto.Id,
                                Date = DateOnly.FromDateTime(DateTime.Now),
                                Time = TimeOnly.FromDateTime(DateTime.Now),
                                BeforeStock = stockAnterior,
                                AfterStock = producto.Stock,
                                RazonCambioAuto = "Devolución de productos (Devolución)",

                                DescriptionCambio = "Devolución procesado y stock actualizado"
                            };
                            _context.Add(historial);

                        }


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
                                    reporte.ComentaryUser = $"Descripción automática: Se agregó {detalle.CantidadProduct} al stock del producto por medio de la sección 'Devolución' con el id {detalle.IdDevolucion}.";
                                    reporte.EndDate = DateOnly.FromDateTime(DateTime.Now);
                                    reporte.EndTime = TimeOnly.FromDateTime(DateTime.Now);

                                    _context.Update(reporte);
                                }
                            }
                        }

                        // Guardar cambios y confirmar transacción
                        _context.SaveChanges();
                        transaction.Commit();
                        TempData["Success"] = "Compra registrada correctamente.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                        TempData["Error"] = "Ocurrió un error al guardar los cambios en la base de datos.";
                        ViewBag.Productos = _context.modelProduct.ToList();
                    return RedirectToAction("Create", "Devolucion");
                }
                }
            }




        [HttpGet]
        public IActionResult DescargarDevolucionesPdf(DateOnly? startDate, DateOnly? endDate)
        {
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            var devoluciones = _context.modelDevolucion
                .Include(c => c.DetalleDevolucion)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                devoluciones = devoluciones.Where(c => c.Date >= startDate && c.Date <= endDate);
            }

            var devolucionesList = devoluciones.ToList();
            if (!devolucionesList.Any())
            {
                TempData["Error"] = "No se encontraron devoluciones en el rango de fechas especificado.";
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



                // Determinar título dinámico
                var title = startDate.HasValue && endDate.HasValue
                    ? $"Devoluviones del {startDate} al {endDate}"
                    : "Todas las Devoluciones";
                // Celda del título
                Cell titleCell = new Cell().Add(new Paragraph(title)
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                var table = new Table(new float[] { 1, 2, 2, 2, 2, 2}).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);
            
                    foreach (var header in new[] { "ID", "Venta Id", "Usuario", "Client/Desc", "Fecha", "Rembolso" })
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
                foreach (var devolucion in devolucionesList)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.Id.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.IdFactura.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.NameUser)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.NameClient ?? "-")).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                     table.AddCell(new Cell().Add(new Paragraph(devolucion.Date.ToShortDateString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{devolucion.DetalleDevolucion.Sum(d => d.PriceTotalReembolso):C}"))
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
                return File(stream.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}_{fechaDescarga}.pdf");
            }
        }


        [HttpGet]
        public IActionResult DescargarDetallesDevolucionPdf(int id)
        {
            var devolucion = _context.modelDevolucion
                .Include(c => c.DetalleDevolucion)
                .FirstOrDefault(c => c.Id == id);

            if (devolucion == null)
            {
                TempData["Error"] = "No se encontró la devolucion especificada.";
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
                Cell titleCell = new Cell().Add(new Paragraph("Detalles de la devolucion")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                document.Add(new Paragraph($"Devolución ID: {devolucion.Id}\n" +
                                           $"Venta ID: {devolucion.IdFactura}\n" +
                                           $"Sucursal: {devolucion.NameSucursal}\n" +
                                           $"Usuario: {devolucion.NameUser}\n" +
                                           $"Fecha: {devolucion.Date.ToString("dd-MM-yyyy")}\n" +
                                           $"Cliente/Descripción: {devolucion.NameClient ?? "-"}")
                    .SetFontSize(12)
                    .SetMarginBottom(15));

                var table = new Table(new float[] { 2, 2, 1, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);

                foreach (var header in new[] { "Producto", "Código", "Cant.", "Reembolso", "Total", "Actualizado stock" })
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
                foreach (var detalle in devolucion.DetalleDevolucion)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(detalle.NameProduct)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceReembolso:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceTotalReembolso:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.StockD ? "Sí" : "No"))
                        .SetFontColor(detalle.StockD ? ColorConstants.GREEN : ColorConstants.RED)
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));
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
                return File(stream.ToArray(), "application/pdf", $"Devolucion_{devolucion.Id}_{fechaDescarga}.pdf");
            }
        }



        private bool ModelDevolucionExists(int id)
        {
            return _context.modelDevolucion.Any(e => e.Id == id);
        }
    }
}
