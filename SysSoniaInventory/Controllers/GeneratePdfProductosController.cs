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

namespace SysSoniaInventory.Controllers
{
    public class GeneratePdfDevolucionController : Controller
    {
        private readonly DBContext _context;

        public GeneratePdfDevolucionController(DBContext context)
        {
            _context = context;
        }

        public IActionResult GeneratePdf(int devolucionId)
        {
            var devolucion = _context.modelDevolucion
                .Where(d => d.Id == devolucionId)
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

                // Agregar logo
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100);
                    logo.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    document.Add(logo);
                }

                // Título estilizado
                document.Add(new Paragraph("Detalles de la Devolución")
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetMarginBottom(10));

                // Información de la devolución
                document.Add(new Paragraph($"Devolución ID: {devolucion.Id}\nFactura ID: {devolucion.IdFactura}\nSucursal: {devolucion.NameSucursal}\nUsuario: {devolucion.NameUser}\nCliente: {devolucion.NameClient}\nFecha: {devolucion.Date:yyyy-MM-dd}")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginBottom(10));

                // Tabla de productos devueltos
                var table = new Table(new float[] { 3, 2, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados estilizados
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

                // Filas alternadas
                var alternateRowColor = new DeviceRgb(230, 240, 255);
                bool isAlternate = false;
                foreach (var detalle in devolucion.DetalleDevolucion)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;

                    table.AddCell(new Cell().Add(new Paragraph(detalle.NameProduct))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CodigoProducto))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.CantidadProduct.ToString()))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.PriceReembolso.ToString("C")))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.PriceTotalReembolso.ToString("C")))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(detalle.StockD ? "Sí" : "No"))
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));

                    isAlternate = !isAlternate;
                }

                // Total General
                var totalGeneral = devolucion.DetalleDevolucion.Sum(d => d.PriceTotalReembolso);
                table.AddCell(new Cell(1, 4).Add(new Paragraph("Total General:").SetBold())
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPadding(5));
                table.AddCell(new Cell(1, 2).Add(new Paragraph(totalGeneral.ToString("C")))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5));

                document.Add(table);

                // Pie de página
                document.Add(new Paragraph("Muebles y Electrodomésticos Sonia")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(20));

                document.Close();

                return File(stream.ToArray(), "application/pdf", $"Devolucion_{devolucion.Id}.pdf");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
