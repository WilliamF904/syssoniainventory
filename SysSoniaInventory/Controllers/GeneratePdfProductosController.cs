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
    public class GeneratePdfProductosController : Controller
    {
        private readonly DBContext _context;

        public GeneratePdfProductosController(DBContext context)
        {
            _context = context;
        }

        // Método para generar PDF de todos los productos
        public IActionResult GeneratePdfAll()
        {
            var productos = _context.modelProduct.ToList();
            return GeneratePdf(productos, "Todos los Productos");
        }

        // Método para generar PDF de productos activos
        public IActionResult GeneratePdfActivos()
        {
            var productos = _context.modelProduct.Where(p => p.Estatus == 1).ToList();
            return GeneratePdf(productos, "Productos Activos");
        }

        // Método para generar PDF de productos inactivos
        public IActionResult GeneratePdfInactivos()
        {
            var productos = _context.modelProduct.Where(p => p.Estatus == 0).ToList();
            return GeneratePdf(productos, "Productos Inactivos");
        }

        // Método general para generar PDF
        private IActionResult GeneratePdf(IEnumerable<ModelProduct> productos, string titulo)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Agregar logo
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "2_ventilador.png");
                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100);
                    document.Add(logo);
                }

                // Título estilizado
                document.Add(new Paragraph(titulo)
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetMarginBottom(20));

                // Crear tabla
                var table = new Table(new float[] { 1, 3, 2, 2, 2, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados estilizados
                var headerColor = new DeviceRgb(41, 128, 185); // Azul oscuro
                foreach (var header in new[] { "ID", "Nombre", "Precio Compra", "Precio Venta", "Stock", "Estatus" })
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
                foreach (var producto in productos)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(producto.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(producto.Name))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(producto.PurchasePrice.ToString("F2")))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(producto.SalePrice.ToString("F2")))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(producto.Stock.ToString()))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(producto.Estatus == 1 ? "Activo" : "Inactivo"))
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

                // Retornar archivo PDF
                return File(stream.ToArray(), "application/pdf", $"{titulo.Replace(" ", "_")}.pdf");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
