using System.IO;
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
using System.Linq;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class BrandController : Controller
{
    private readonly DBContext _context;

    public BrandController(DBContext context)
    {
        _context = context;
    }

    // Método para generar PDF de todas las marcas
    public IActionResult GeneratePdfMarca()
    {
        // Verificar niveles de acceso
        if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
        {
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
            return RedirectToAction("Index", "Home");
        }

        // Obtener la lista de marcas
        var brands = _context.Set<ModelMarca>().ToList();
        if (!brands.Any())
        {
            TempData["Error"] = "No hay marcas disponibles para generar el PDF.";
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

            // Título estilizado
            document.Add(new Paragraph("Lista de Marcas")
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetMarginBottom(20));

            // Crear tabla
            var table = new Table(new float[] { 1, 3, 5 }).SetWidth(UnitValue.CreatePercentValue(100));
            table.SetMarginTop(10);

            // Encabezados
            var headerColor = new DeviceRgb(41, 128, 185); // Azul oscuro
            foreach (var header in new[] { "ID", "Nombre", "Descripción" })
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
            foreach (var brand in brands)
            {
                var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                table.AddCell(new Cell().Add(new Paragraph(brand.Id.ToString()))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(brand.Name))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(brand.Description ?? "Sin descripción"))
                    .SetBackgroundColor(rowColor));
                isAlternate = !isAlternate;
            }

            document.Add(table);

            // Pie de página
            document.Add(new Paragraph("Muebles y Electrodomésticos Sonia")
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(20));

            document.Close();

            // Retornar archivo PDF
            return File(stream.ToArray(), "application/pdf", "Lista_Marcas.pdf");
        }
    }
}
