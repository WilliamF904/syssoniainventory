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
public class PdfCompraDetallesController(DBContext context) : Controller
{
    private readonly DBContext _context = context;

    [HttpGet]
    public IActionResult DescargarCompraPdf(int id)
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

            document.Add(new Paragraph($"Sucursal: {compra.NameSucursal}\n" +
                                       $"Usuario: {compra.NameUser}\n" +
                                       $"Proveedor: {compra.NameProveedor ?? "-"}\n" +
                                       $"Factura: {compra.CodigoFactura ?? "-"}\n" +
                                       $"Fecha: {compra.Date.ToString("dd-MM-yyyy")}\n" +
                                       $"Descripción: {compra.Description ?? "-"}")
                .SetFontSize(12)
                .SetMarginBottom(15));

            var table = new Table(new float[] { 2, 2, 2, 1, 2, 2, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
            var headerColor = new DeviceRgb(41, 128, 185);

            foreach (var header in new[] { "Producto", "Marca", "Código", "Cant.", "Precio Compra", "Total", "Actualizado en stock" })
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
                table.AddCell(new Cell().Add(new Paragraph(detalle.NameProducto)).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(detalle.MarcaProducto)).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto)).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString())).SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceCompraUnitario:C}"))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph($"{detalle.PriceTotal:C}"))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
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
}
