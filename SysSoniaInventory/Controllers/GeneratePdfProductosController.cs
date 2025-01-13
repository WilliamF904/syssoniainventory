using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using System.Linq;
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

                // Título
                document.Add(new Paragraph(titulo)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                // Crear tabla
                var table = new Table(new float[] { 1, 3, 2, 2, 2, 1 });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Encabezados
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Precio Compra");
                table.AddHeaderCell("Precio Venta");
                table.AddHeaderCell("Stock");
                table.AddHeaderCell("Estatus");

                // Agregar datos
                foreach (var producto in productos)
                {
                    table.AddCell(producto.Id.ToString());
                    table.AddCell(producto.Name);
                    table.AddCell(producto.PurchasePrice.ToString("F2"));
                    table.AddCell(producto.SalePrice.ToString("F2"));
                    table.AddCell(producto.Stock.ToString());
                    table.AddCell(producto.Estatus == 1 ? "Activo" : "Inactivo");
                }

                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", $"{titulo.Replace(" ", "_")}.pdf");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
