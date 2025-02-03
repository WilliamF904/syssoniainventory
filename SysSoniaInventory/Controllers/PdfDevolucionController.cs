using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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

namespace SysSoniaInventory.Controllers
{
    public class PdfDevolucionController : Controller
    {
        private readonly DBContext _context;

        public PdfDevolucionController(DBContext context)
        {
            _context = context;
        }

        // Descargar todas las devoluciones
        public IActionResult DescargarTodasLasDevolucionesPdf()
        {
            var devoluciones = _context.modelDevolucion.ToList();
            return GenerarPdf(devoluciones, "Todas las Devoluciones");
        }

        // Descargar devoluciones por fecha
        public IActionResult DescargarDevolucionesPorFechaPdf(DateTime fechaInicio, DateTime fechaFin)
        {
            var fechaInicioDateOnly = DateOnly.FromDateTime(fechaInicio);
            var fechaFinDateOnly = DateOnly.FromDateTime(fechaFin);

            var devoluciones = _context.modelDevolucion
                .Where(d => d.Date >= fechaInicioDateOnly && d.Date <= fechaFinDateOnly)
                .ToList();

            return GenerarPdf(devoluciones, $"Devoluciones del {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}");
        }


        // Método general para generar el PDF
        private IActionResult GenerarPdf(IEnumerable<ModelDevolucion> devoluciones, string titulo)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Crear una tabla para el encabezado (logo y título en la misma línea)
                var headerTable = new Table(new float[] { 1, 3 }).SetWidth(UnitValue.CreatePercentValue(100));
                headerTable.SetMarginBottom(10);

                // Agregar logo si existe
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(80, 80);
                    var logoCell = new Cell().Add(logo).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER)); // Espacio vacío si no hay logo
                }

                // Agregar título estilizado
                var titleCell = new Cell()
                    .Add(new Paragraph(titulo)
                        .SetFontSize(20)
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBold())
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                headerTable.AddCell(titleCell);

                // Agregar la tabla de encabezado al documento
                document.Add(headerTable);


                // Crear tabla
                var table = new Table(new float[] { 1, 2, 2, 2, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados
                var headerColor = new DeviceRgb(41, 128, 185);
                foreach (var header in new[] { "ID", "Factura", "Sucursal", "Usuario", "Cliente", "Fecha" })
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                            .SetFontColor(ColorConstants.WHITE)
                            .SetBold())
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(8));
                }

                // Filas alternadas
                var alternateRowColor = new DeviceRgb(230, 240, 255);
                bool isAlternate = false;
                foreach (var devolucion in devoluciones)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.IdFactura.ToString()))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.NameSucursal))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.NameUser))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.NameClient ?? "N/A"))
                        .SetBackgroundColor(rowColor));
                    table.AddCell(new Cell().Add(new Paragraph(devolucion.Date.ToString("dd/MM/yyyy")))
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

                return File(stream.ToArray(), "application/pdf", $"{titulo.Replace(" ", "_")}.pdf");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
