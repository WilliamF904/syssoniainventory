using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CategoryController : Controller
{
    private readonly DBContext _context;

    public CategoryController(DBContext context)
    {
        _context = context;
    }

    // Método para generar PDF de todas las categorías
    public IActionResult GeneratePdfCategoria()
    {     // Verificar niveles de acceso
        if (User.HasClaim("AccessTipe", "Nivel 4"))
        { // Nivel 4 tiene acceso

        }
        else
        {
            // Redirigir con mensaje de error si el usuario no tiene acceso
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
            return RedirectToAction("Index", "Home");
        }
        // Obtener la lista de categorías
        var categories = _context.Set<ModelCategory>().ToList();

        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título
            document.Add(new Paragraph("Lista de Categorías")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            // Crear tabla
            var table = new Table(new float[] { 1, 2 }); // Ajustar columnas según los datos
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Encabezados
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Nombre");

            // Agregar datos de las categorías
            foreach (var category in categories)
            {
                table.AddCell(category.Id.ToString());
                table.AddCell(category.Name);
            }

            document.Add(table);
            document.Close();

            // Retornar archivo PDF
            return File(stream.ToArray(), "application/pdf", "Categorias.pdf");
        }
    }


}
