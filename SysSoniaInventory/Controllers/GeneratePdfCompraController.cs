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
public class ComprasController : Controller
{
    private readonly DBContext _context;

    public ComprasController(DBContext context)
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

        var compras = _context.modelDetalleCompra
            .Include(c => c.IdCompraNavigation)
            .AsQueryable();

        if (startDate.HasValue && endDate.HasValue)
        {
            compras = compras.Where(c => c.IdCompraNavigation.Date >= startDate && c.IdCompraNavigation.Date <= endDate);
        }

        var comprasList = compras.ToList();
        if (!comprasList.Any())
        {
            TempData["Error"] = "No se encontraron compras en el rango de fechas especificado.";
            return RedirectToAction("Index");
        }

        decimal totalGeneral = comprasList.Sum(c => c.PriceTotal);

        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var headerTable = new Table(new float[] { 1, 4 }).SetWidth(UnitValue.CreatePercentValue(100));
            headerTable.SetBorder(Border.NO_BORDER);

            var logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
            var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100).SetMarginTop(-50);
            logoCell.Add(logo);

            var titleCell = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
            string title = startDate.HasValue && endDate.HasValue
                ? $"Compras del {startDate} al {endDate}"
                : "Todas las Compras";
            titleCell.Add(new Paragraph(title)
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetBold()
                .SetMarginBottom(10));

            headerTable.AddCell(logoCell);
            headerTable.AddCell(titleCell);

            document.Add(headerTable);

            document.Add(new Paragraph($"Total General de Compras: {totalGeneral:C}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginBottom(20));

            var table = new Table(new float[] { 1, 2, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
            var headerColor = new DeviceRgb(41, 128, 185);
            foreach (var header in new[] { "ID", "Producto", "Marca", "Cantidad", "Total", "Fecha de Compra" })
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
                table.AddCell(new Cell().Add(new Paragraph(compra.Id.ToString()))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(compra.NameProducto))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.MarcaProducto ?? "-"))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(compra.CantidadProduct.ToString()))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph($"{compra.PriceTotal:C}"))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph(compra.IdCompraNavigation.Date.ToShortDateString()))
                    .SetBackgroundColor(rowColor));

                isAlternate = !isAlternate;
            }

            document.Add(table);

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
