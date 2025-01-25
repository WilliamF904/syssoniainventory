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

        var facturas = _context.Set<ModelFactura>().AsQueryable();
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

        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Agregar logo una sola vez  
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "2_ventilador.png");
            var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100);

            document.Add(logo); // Agregar el logo aquí  

            // Título estilizado  
            string title = startDate.HasValue && endDate.HasValue
                ? $"Facturas del {startDate} al {endDate}"
                : "Todas las Facturas";

            document.Add(new Paragraph(title)
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetMarginBottom(20));

            // Crear tabla  
            var table = new Table(new float[] { 1, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
            table.SetMarginTop(10);

            // Encabezados  
            var headerColor = new DeviceRgb(41, 128, 185); // Azul oscuro  
            foreach (var header in new[] { "ID", "Sucursal", "Usuario", "Cliente", "Fecha" })
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetBold())
                    .SetBackgroundColor(headerColor)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(8));
            }

            // Filas alternadas  
            var alternateRowColor = new DeviceRgb(230, 240, 255); // Azul claro  
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
            return File(stream.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}.pdf");
        }
    }
}