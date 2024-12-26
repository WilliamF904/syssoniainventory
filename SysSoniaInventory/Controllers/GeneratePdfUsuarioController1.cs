using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;

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
        if (User.HasClaim("AccessTipe", "Nivel 4"))
        { // Nivel 4 tiene acceso

        }
        else
        {
            // Redirigir con mensaje de error si el usuario no tiene acceso
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
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

            // Título
            document.Add(new Paragraph("Lista de Usuarios")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            // Crear tabla
            var table = new Table(new float[] { 1, 2, 2, 2, 1 });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Encabezados
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Nombre");
            table.AddHeaderCell("Apellido");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("Estado");

            // Agregar datos
            foreach (var user in userList)
            {
                table.AddCell(user.Id.ToString());
                table.AddCell(user.Name);
                table.AddCell(user.LastName);
                table.AddCell(user.Email);
                table.AddCell(user.Estatus == 1 ? "Activo" : "Inactivo");
            }

            document.Add(table);
            document.Close();

            // Retornar archivo PDF
            return File(stream.ToArray(), "application/pdf", "Usuarios.pdf");
        }
    }

}
