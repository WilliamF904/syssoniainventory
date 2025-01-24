using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.IO.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class GeneratePdfProveedoresController : Controller
    {
        private readonly DBContext _context;

        public GeneratePdfProveedoresController(DBContext context)
        {
            _context = context;
        }

        // Método para generar PDF de proveedores
        public IActionResult GeneratePdf()
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener datos de proveedores
            var proveedores = _context.modelProveedor.ToList();

            // Generar PDF
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Agregar logo
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100);
                    document.Add(logo);
                }

                // Título estilizado
                document.Add(new Paragraph("Lista de Proveedores")
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetMarginBottom(20));

                // Crear tabla
                var table = new Table(new float[] { 1, 3, 3, 2, 3 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados estilizados
                var headerColor = new DeviceRgb(41, 128, 185); // Azul oscuro
                foreach (var header in new[] { "ID", "Nombre", "Descripción", "Teléfono", "Email" })
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
                foreach (var proveedor in proveedores)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(proveedor.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(proveedor.Name))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(proveedor.Description ?? "N/A"))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(proveedor.Tel.HasValue ? proveedor.Tel.Value.ToString() : "N/A"))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(proveedor.Email ?? "N/A"))
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
                return File(stream.ToArray(), "application/pdf", "Proveedores.pdf");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

