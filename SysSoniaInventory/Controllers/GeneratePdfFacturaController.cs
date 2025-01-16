using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

[Authorize]
public class FacturaController : Controller
{
    private readonly DBContext _context;

    public FacturaController(DBContext context)
    {
        _context = context;
    }

    // Método para generar PDF de facturas según fecha o todas
    public IActionResult GeneratePdf(DateOnly? startDate, DateOnly? endDate)
    {
        // Verificar acceso del usuario
        if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
        {
            TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
            return RedirectToAction("Index", "Home");
        }

        // Filtrar facturas por rango de fecha o incluir todas
        var facturas = _context.Set<ModelFactura>().AsQueryable();

        if (startDate.HasValue && endDate.HasValue)
        {
            facturas = facturas.Where(f => f.Date >= startDate && f.Date <= endDate);
        }

        var facturasList = facturas.ToList();

        // Manejar error si no se encuentran facturas
        if (!facturasList.Any())
        {
            TempData["Error"] = "No se encontraron facturas en el rango de fechas especificado.";
            return RedirectToAction("Index");
        }

        using (var stream = new MemoryStream())
        {
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título del documento
            string title = startDate.HasValue && endDate.HasValue
                ? $"Facturas del {startDate} al {endDate}"
                : "Todas las Facturas";

            document.Add(new Paragraph(title)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            // Crear tabla para las facturas
            var table = new Table(new float[] { 1, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Sucursal");
            table.AddHeaderCell("Usuario");
            table.AddHeaderCell("Cliente");
            table.AddHeaderCell("Fecha");

            // Agregar datos
            foreach (var factura in facturasList)
            {
                table.AddCell(factura.Id.ToString());
                table.AddCell(factura.NameSucursal);
                table.AddCell(factura.NameUser);
                table.AddCell(factura.NameClient ?? "N/A");
                table.AddCell(factura.Date.ToShortDateString());
            }

            document.Add(table);
            document.Close();

            // Descargar archivo
            return File(stream.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}.pdf");
        }
    }
}
        