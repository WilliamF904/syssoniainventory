﻿using System;
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
    public class CompraController : Controller
    {
        private readonly DBContext _context;

        public CompraController(DBContext context)
        {
            _context = context;
        }

        // GET: Compra
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

            int pageSize = 5; // Número de compras por página
            var comprasQuery = _context.modelCompra
                .Include(f => f.DetalleCompra)
                .AsQueryable();

            // Filtrar  compras según el nivel de acceso
            if (accessLevel == "Nivel 2" || accessLevel == "Nivel 3")
            {
                comprasQuery = comprasQuery.Where(f => f.NameUser == userName);
            }

            // Aplicar filtros de búsqueda
            if (!string.IsNullOrEmpty(searchUser))
            {
                comprasQuery = comprasQuery.Where(f => f.NameUser.Contains(searchUser));
            }

            if (startDate.HasValue)
            {
                comprasQuery = comprasQuery.Where(f => f.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                comprasQuery = comprasQuery.Where(f => f.Date <= endDate.Value);
            }

            // Ordenar y paginar los resultados
            var compras = await comprasQuery
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Preparar datos para la vista
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)comprasQuery.Count() / pageSize);
            ViewBag.SearchUser = searchUser;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(compras);
        }





        // GET: Compra/Details/5
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
            // Obtener la compra y sus detalles
            var compra = _context.modelCompra
                .Include(f => f.DetalleCompra) // Incluir los detalles de la compra
                .FirstOrDefault(f => f.Id == id);

            if (compra == null)
            {
                TempData["Error"] = "Compra no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar el nivel de acceso
            var accessLevel = User.Claims.FirstOrDefault(c => c.Type == "AccessTipe")?.Value;
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Si es Nivel 2 o Nivel 3, verificar que la compra le corresponda
            if ((accessLevel == "Nivel 2" || accessLevel == "Nivel 3") && compra.NameUser != userName)
            {
                TempData["Error"] = "No tienes permisos para acceder a esta compra.";
                return RedirectToAction(nameof(Index));
            }


            // Pasar los datos a la vista
            return View(compra);
        }





        // GET: Compra/Create
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
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Name", "Name");

            ViewBag.Productos = _context.modelProduct
           .Include(p => p.IdMarcanavigation)  // Aquí debe coincidir con la propiedad de navegación
           .ToList();

            return View();
        }

        // Acción Create para manejar el envío del formulario
        [HttpPost]
        public IActionResult Create(ModelCompra compra, List<ModelDetalleCompra> detalles)
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
            compra.NameUser = User.Identity?.Name;
            compra.NameSucursal = User.FindFirst("Sucursal")?.Value;
            compra.Date = DateOnly.FromDateTime(DateTime.Now);
            compra.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Validar que los datos son correctos
            ModelState.Remove("Description");
            if (compra.Description == null)
            {
                compra.Description = "";
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error inesperado en la validación de un campo o más.";
                ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                ViewBag.NameUser = User.Identity?.Name;

                ViewBag.Productos = _context.modelProduct.ToList();
                return View(compra);
            }

            if (detalles == null || !detalles.Any())
            {
                TempData["Error"] = "Debe agregar al menos un detalle a la compra.";
                ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                ViewBag.NameUser = User.Identity?.Name;

                ViewBag.Productos = _context.modelProduct.ToList();
                return View(compra);
            }

            foreach (var detalle in detalles)
            {
                if (detalle.CantidadProduct <= 0)
                {
                    TempData["Error"] = "La cantidad de un producto debe ser mayor a cero.";
                    ViewBag.NameSucursal = User.FindFirst("Sucursal")?.Value;
                    ViewBag.NameUser = User.Identity?.Name;

                    ViewBag.Productos = _context.modelProduct.ToList();
                    return View(compra);
                }
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Guardar la compra
                    _context.modelCompra.Add(compra);
                    _context.SaveChanges(); // Aquí se genera el Id de la compra

                    foreach (var detalle in detalles)
                    {
                        // Obtener los datos actuales del producto seleccionado
                        var producto = _context.modelProduct.FirstOrDefault(p => p.Id == detalle.IdProduct);
                        if (producto == null)
                        {
                            TempData["Error"] = $"El producto con ID {detalle.IdProduct} no existe.";
                            ViewBag.Productos = _context.modelProduct.ToList();
                            transaction.Rollback();
                            return View(compra);
                        }
                        // Verifica si se debe actualizar el precio
                        var beforePurchasePrice = producto.PurchasePrice;
                        if (detalle.UpdatePrice)
                        {
                            // Guardar el precio anterior (BeforePurchasePrice)
                          
                            producto.PurchasePrice = detalle.PriceCompraUnitario;  // Actualizas el precio
                                    _context.Update(producto);

                        }

                        // Almacenar valores originales para el historial
                        var stockAnterior = producto.Stock;

                        // Actualizar stock sumando la cantidad de productos comprados
                        producto.Stock += detalle.CantidadProduct;

                        // Asignar nombre de la marca
                        var nombreMarca = _context.modelMarca
                            .Where(m => m.Id == producto.IdMarca)
                            .Select(m => m.Name)
                            .FirstOrDefault();

                        detalle.MarcaProducto = nombreMarca ?? "Marca desconocida";

                        // Asignar valores al detalle de compra
                        detalle.IdCompra = compra.Id;
                        detalle.CodigoProducto = producto.Codigo;
                        detalle.NameProducto = producto.Name;
                        detalle.PriceCompraUnitario = detalle.PriceCompraUnitario; // Usar el precio ingresado por el usuario                                                                
                        detalle.PriceTotal = detalle.PriceCompraUnitario * detalle.CantidadProduct;
                        detalle.UpdatePrice = detalle.UpdatePrice;
                        // Guardar el detalle
                        _context.modelDetalleCompra.Add(detalle);

                        // Guardar el historial del cambio de stock
                        var historial = new ModelHistorialProduct
                        {
                            NameUser = User.Identity?.Name,
                            IdProduct = producto.Id,
                            Date = DateOnly.FromDateTime(DateTime.Now),
                            Time = TimeOnly.FromDateTime(DateTime.Now),
                            BeforeStock = stockAnterior,
                            AfterStock = producto.Stock,
                            BeforePurchasePrice = detalle.UpdatePrice ? beforePurchasePrice : (decimal?)null,  // Solo llenamos si se actualiza el precio
                            AfterPurchasePrice = detalle.UpdatePrice ? producto.PurchasePrice : (decimal?)null, // Solo llenamos si se actualiza el precio
                            RazonCambioAuto = "Entrada de productos (Compra)",
                            DescriptionCambio = "Compra registrada y stock actualizado"
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
                                reporte.ComentaryUser = $"Descripción automática: Se agregó {detalle.CantidadProduct} al stock del producto por medio de la sección 'Compra' con el id {detalle.IdCompra}.";
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
                    return View(compra);
                }
            }
        }






        [HttpGet]
        public IActionResult DescargarComprasPdf(DateOnly? startDate, DateOnly? endDate)
        {
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            var compras = _context.modelCompra
                .Include(c => c.DetalleCompra)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                compras = compras.Where(c => c.Date >= startDate && c.Date <= endDate);
            }

            var comprasList = compras.ToList();
            if (!comprasList.Any())
            {
                TempData["Error"] = "No se encontraron compras en el rango de fechas especificado.";
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
                    ? $"Compras del {startDate} al {endDate}"
                    : "Todas las Compras";
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

                var table = new Table(new float[] { 1, 2, 2, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);

                foreach (var header in new[] { "ID", "Sucursal", "Usuario", "Proveedor", "Código Factura", "Fecha", "Total" })
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
                foreach (var compra in comprasList)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(compra.Id.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(compra.NameSucursal)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(compra.NameUser)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(compra.NameProveedor ?? "-")).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(compra.CodigoFactura ?? "-")).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(compra.Date.ToShortDateString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{compra.DetalleCompra.Sum(d => d.PriceTotal):C}"))
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
        public IActionResult DescargarDetallesCompraPdf(int id)
        {
            var compra = _context.modelCompra
                .Include(c => c.DetalleCompra)
                .FirstOrDefault(c => c.Id == id);

            if (compra == null)
            {
                TempData["Error"] = "No se encontró la compra especificada.";
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
                Cell titleCell = new Cell().Add(new Paragraph("Detalles de la Compra")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                document.Add(new Paragraph($"Compra ID: {compra.Id}\n" + 
                    $"Sucursal: {compra.NameSucursal}\n" +
                                           $"Usuario: {compra.NameUser}\n" +
                                           $"Proveedor: {compra.NameProveedor ?? "-"}\n" +
                                           $"Factura: {compra.CodigoFactura ?? "-"}\n" +
                                           $"Fecha: {compra.Date.ToString("dd-MM-yyyy")}\n" +
                                           $"Descripción: {compra.Description ?? "-"}")
                    .SetFontSize(12)
                    .SetMarginBottom(15));

                var table = new Table(new float[] { 2, 2, 2, 1, 1, 2, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);

                foreach (var header in new[] { "Producto", "Marca", "Código", "Cant.", "Precio Compra", "Total", "Precio actualizado" })
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
                foreach (var detalle in compra.DetalleCompra)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(detalle.NameProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.MarcaProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto)).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString())).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceCompraUnitario:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceTotal:C}"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.UpdatePrice ? "Sí" : "No"))
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
                return File(stream.ToArray(), "application/pdf", $"Compra_{compra.Id}_{fechaDescarga}.pdf");
            }
        }

        private bool ModelCompraExists(int id)
        {
            return _context.ModelCompra.Any(e => e.Id == id);
        }
    }
}
