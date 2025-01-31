using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.IO.Image;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class FacturaController : Controller
{
    private readonly DBContext _context;

    public FacturaController(DBContext context)
    {
        _context = context;
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
        decimal gananciaTotal =  totalGeneral  - totalVentaBruta;

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
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(factura.NameUser))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(factura.NameClient ?? "N/A"))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(factura.Date.ToShortDateString()))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph($"{factura.TotalVenta:C}")) // Mostrar el total de la factura
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));

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

}