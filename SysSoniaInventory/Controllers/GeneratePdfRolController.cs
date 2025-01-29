using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using iText.IO.Image;

[Authorize]
public class RolController : Controller
{
    private readonly DBContext _context;

    public RolController(DBContext context)
    {
        _context = context;
    }

    // Método para generar PDF  
    public IActionResult GeneratePdf(bool? active = null)
    {
        // Verificar niveles de acceso  
        if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
        {
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
            return RedirectToAction("Index", "Home");
        }

        // Obtener datos de roles (filtrar por estado si se especifica)  
        var roles = _context.modelRol.AsQueryable();

        if (active.HasValue)
        {
            roles = roles.Where(r => r.User.Any(u => u.Estatus == (active.Value ? 1 : 0)));
        }

        var roleList = roles.ToList();

        // Generar PDF  
        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Ruta del logo  
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");

            // Verificar si el archivo del logo existe  
            if (System.IO.File.Exists(imagePath))
            {
                var logo = new Image(ImageDataFactory.Create(imagePath))
                    .ScaleAbsolute(100, 100); // Tamaño del logo  
                    
                    
                document.Add(logo);
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el logo.";
            }

            // Título estilizado  
            document.Add(new Paragraph("Lista de Roles")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetBold()
                .SetMarginBottom(20));

            // Crear tabla  
            var table = new Table(new float[] { 1, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
            table.SetMarginTop(10);

            // Encabezados estilizados  
            var headerColor = new DeviceRgb(52, 152, 219); // Azul intenso  
            foreach (var header in new[] { "ID", "Nombre", "Tipo de Acceso" })
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetBold())
                    .SetBackgroundColor(headerColor)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(8));
            }

            // Filas alternadas  
            var alternateRowColor = new DeviceRgb(235, 245, 255); // Azul claro  
            bool isAlternate = false;
            foreach (var role in roleList)
            {
                var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                table.AddCell(new Cell().Add(new Paragraph(role.Id.ToString()))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(role.Name))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(role.AccessTipe))
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
            return File(stream.ToArray(), "application/pdf", "Roles.pdf");
        }
    }
}
