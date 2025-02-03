using System.IO;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.IO.Image;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Controllers
{
    public class GeneratePdfDevolucionDetallesController : Controller
    {
        private readonly DBContext _context;

        public GeneratePdfDevolucionDetallesController(DBContext context)
        {
            _context = context;
        }

        public IActionResult GeneratePdf(int devolucionId)
        {
            var devolucion = _context.modelDevolucion
                .Where(d => d.Id == devolucionId)
                .Include(d => d.DetalleDevolucion)
                .FirstOrDefault();

            if (devolucion == null)
            {
                return NotFound();
            }

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                var headerTable = new Table(new float[] { 1, 3 }).UseAllAvailableWidth();

                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(80, 80);
                    headerTable.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER));
                }
                else
                {
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }

                headerTable.AddCell(new Cell().Add(new Paragraph("Detalles de la Devolución")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));

                document.Add(headerTable);

                document.Add(new Paragraph($"Devolución ID: {devolucion.Id}\n" +
                                           $"Factura ID: {devolucion.IdFactura}\n" +
                                           $"Sucursal: {devolucion.NameSucursal}\n" +
                                           $"Usuario: {devolucion.NameUser}\n" +
                                           $"Cliente: {devolucion.NameClient}\n" +
                                           $"Fecha: {devolucion.Date:dd-MM-yyyy}")
                    .SetFontSize(12)
                    .SetMarginBottom(15));

                var table = new Table(new float[] { 3, 2, 1, 2, 2, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);

                foreach (var header in new[] { "Producto", "Código", "Cantidad", "Precio Reembolso", "Total Reembolso", "Stock Devuelto" })
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
                decimal totalGeneral = 0;

                foreach (var detalle in devolucion.DetalleDevolucion)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    totalGeneral += detalle.PriceTotalReembolso;

                    table.AddCell(new Cell().Add(new Paragraph(detalle.NameProduct ?? "N/A")).SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto ?? "N/A")).SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString())).SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.PriceReembolso.ToString("C"))).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.PriceTotalReembolso.ToString("C"))).SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.RIGHT));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.StockD ? "Sí" : "No"))
                        .SetFontColor(detalle.StockD ? ColorConstants.GREEN : ColorConstants.RED)
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));

                    isAlternate = !isAlternate;
                }

                table.AddCell(new Cell(1, 4).Add(new Paragraph("Total General:").SetBold()).SetBackgroundColor(headerColor).SetFontColor(ColorConstants.WHITE));
                table.AddCell(new Cell(1, 2).Add(new Paragraph(totalGeneral.ToString("C"))).SetTextAlignment(TextAlignment.RIGHT).SetBackgroundColor(headerColor).SetFontColor(ColorConstants.WHITE));

                document.Add(table);

                document.Add(new Paragraph("Muebles y Electrodomésticos Sonia")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20));

                document.Close();

                return File(stream.ToArray(), "application/pdf", $"Devolucion_{devolucion.Id}.pdf");
            }
        }
    }
}
