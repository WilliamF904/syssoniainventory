using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;

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
        if (User.HasClaim("AccessTipe", "Nivel 4"))
        { // Nivel 4 tiene acceso

        }
        else if (User.HasClaim("AccessTipe", "Nivel 5"))
        {
            // Nivel 5 tiene acceso

        }
        else
        {
            // Redirigir con mensaje de error si el usuario no tiene acceso
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
            return RedirectToAction("Index", "Home");
        }

        // Obtener datos de roles (filtrar por estado si se especifica)
        var roles = _context.modelRol.AsQueryable();

        if (active.HasValue)
        {
            // Suponiendo que "active" indica una propiedad booleana en el modelo
            roles = roles.Where(r => r.User.Any(u => u.Estatus == (active.Value ? 1 : 0)));
        }

        var roleList = roles.ToList();

        // Generar PDF
        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título
            document.Add(new Paragraph("Lista de Roles")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            // Crear tabla
            var table = new Table(new float[] { 1, 2, 2 });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Encabezados
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Nombre");
            table.AddHeaderCell("Tipo de Acceso");

            // Agregar datos
            foreach (var role in roleList)
            {
                table.AddCell(role.Id.ToString());
                table.AddCell(role.Name);
                table.AddCell(role.AccessTipe);
            }

            document.Add(table);
            document.Close();

            // Retornar archivo PDF
            return File(stream.ToArray(), "application/pdf", "Roles.pdf");
        }
    }


}
