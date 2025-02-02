using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using iText.Kernel.Pdf;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using System;
using System.IO;
using System.Linq;

[Authorize]
public class PdfController : Controller
{
    private readonly DBContext _context;

    public PdfController(DBContext context)
    {
        _context = context;
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

            // Agregar logo
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
            if (System.IO.File.Exists(imagePath))
            {
                var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100);
                document.Add(logo);
            }

            var title = startDate.HasValue && endDate.HasValue
                ? $"Compras del {startDate} al {endDate}"
                : "Todas las Compras";
            document.Add(new Paragraph(title)
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

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
                table.AddCell(new Cell().Add(new Paragraph(compra.Id.ToString())).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.NameSucursal)).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.NameUser)).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.NameProveedor ?? "-")).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.CodigoFactura ?? "-")).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.Date.ToShortDateString())).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph($"{compra.DetalleCompra.Sum(d => d.PriceTotal):C}"))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
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
}
