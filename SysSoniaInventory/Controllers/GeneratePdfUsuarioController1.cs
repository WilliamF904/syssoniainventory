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
public class UserController : Controller
{
    private readonly DBContext _context;

    public UserController(DBContext context)
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

        // Obtener datos de usuarios (filtrar por estado si se especifica)
        var users = _context.modelUser.AsQueryable();
        if (active.HasValue)
        {
            users = users.Where(u => u.Estatus == (active.Value ? 1 : 0));
        }

        var userList = users.ToList();

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
            document.Add(new Paragraph("Lista de Usuarios")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(24)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetBold()
                .SetMarginBottom(20));

            // Crear tabla
            var table = new Table(new float[] { 1, 2, 2, 3, 1 }).SetWidth(UnitValue.CreatePercentValue(100));
            table.SetMarginTop(10);



            // Encabezados estilizados
            var headerColor = new DeviceRgb(52, 152, 219); // Azul intenso
            foreach (var header in new[] { "ID", "Nombre", "Apellido", "Email", "Estado" })
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
            foreach (var user in userList)
            {
                var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                table.AddCell(new Cell().Add(new Paragraph(user.Id.ToString()))
                    .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(user.Name))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(user.LastName))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(user.Email))
                    .SetBackgroundColor(rowColor));
                table.AddCell(new Cell().Add(new Paragraph(user.Estatus == 1 ? "Activo" : "Inactivo"))
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
            return File(stream.ToArray(), "application/pdf", "Usuarios.pdf");
        }
    }
}

