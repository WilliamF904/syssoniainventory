using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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

                // Título
                document.Add(new Paragraph("Lista de Proveedores")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                // Crear tabla
                var table = new Table(new float[] { 1, 3, 3, 2, 3 });
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Encabezados
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Descripción");
                table.AddHeaderCell("Teléfono");
                table.AddHeaderCell("Email");

                // Agregar datos
                foreach (var proveedor in proveedores)
                {
                    table.AddCell(proveedor.Id.ToString());
                    table.AddCell(proveedor.Name);
                    table.AddCell(proveedor.Description ?? "N/A");
                    table.AddCell(proveedor.Tel.HasValue ? proveedor.Tel.Value.ToString() : "N/A");
                    table.AddCell(proveedor.Email ?? "N/A");
                }

                document.Add(table);
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
