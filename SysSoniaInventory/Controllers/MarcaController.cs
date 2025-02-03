using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using iText.Layout;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class MarcaController : Controller
    {
        private readonly DBContext _context;
       

        public MarcaController(DBContext context)
        {
            _context = context;
        }

        // GET: Marca
        public async Task<IActionResult> Index()
        {  // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            return View(await _context.modelMarca.ToListAsync());
        }

        // GET: Marca/Details/5
        public async Task<IActionResult> Details(int? id)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelMarca == null)
            {
                return NotFound();
            }

            return View(modelMarca);
        }
        public async Task<IActionResult> ProductosPorMarca(int id, int page = 1)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            // Verificar si la marca existe
            var marca = await _context.modelMarca.FindAsync(id);
            if (marca == null)
            {
                TempData["Error"] = "Debe seleccionar un id de marca valido.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de productos por página

            // Obtener la consulta de productos con las relaciones necesarias
            var query = _context.modelProduct
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Where(p => p.IdMarca == id) // Filtrar por IdMarca
                .AsQueryable();

            // Contar el total de productos para la marca
            int totalProductos = await query.CountAsync();

            // Aplicar paginación
            var productos = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (!productos.Any())
            {
                TempData["reporte"] = $"No se encontraron productos relacionados con la marca '{marca.Name}'.";
            }
            // Pasar datos a la vista
            ViewBag.MarcaId = id;
            ViewBag.MarcaNombre = marca.Name; // Pasar el nombre de la marca
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProductos / pageSize); // Calcular el total de páginas

            return View(productos);
        }

        public async Task<IActionResult> DescargarProductosPorMarca(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar si la marca existe
            var marca = await _context.modelMarca.FindAsync(id);
            if (marca == null)
            {
                TempData["Error"] = "Debe seleccionar un ID de marca válido.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los productos asociados a la marca
            var productos = await _context.modelProduct
                .Where(p => p.IdMarca == id)
                .ToListAsync();

            if (!productos.Any())
            {
                TempData["Error"] = $"No se encontraron productos relacionados con la marca '{marca.Name}'.";
                return RedirectToAction(nameof(ProductosPorMarca), new { id });
            }

            // Generar el PDF
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Crear cabecera con el logo y el título
                var headerTable = new Table(new float[] { 1, 4 }).SetWidth(UnitValue.CreatePercentValue(100));
                headerTable.SetBorder(Border.NO_BORDER);

                // Celda del logo
                var logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
           
                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");

                var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100).SetMarginTop(-50);
                logoCell.Add(logo);

                // Celda del título
                var titleCell = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                titleCell.Add(new Paragraph($"Productos de la Marca: {marca.Name}")
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetMarginBottom(10));

                headerTable.AddCell(logoCell);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                // Crear tabla con los productos (ID, Nombre, Código)
                var table = new Table(new float[] { 1, 3, 2 }).SetWidth(UnitValue.CreatePercentValue(100));

                // Encabezados de la tabla
                var headerColor = new DeviceRgb(41, 128, 185);
                foreach (var header in new[] { "ID", "Nombre del Producto", "Código" })
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
                foreach (var producto in productos)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;

                    table.AddCell(new Cell().Add(new Paragraph(producto.Id.ToString()))
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell().Add(new Paragraph(producto.Name))
                        .SetBackgroundColor(rowColor));

                    table.AddCell(new Cell().Add(new Paragraph(producto.Codigo))
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));

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

                string fechaDescarga = DateTime.Now.ToString("yyyy-MM-dd");
                return File(stream.ToArray(), "application/pdf", $"Productos_{marca.Name.Replace(" ", "_")}_{fechaDescarga}.pdf");
            }
        }


        // GET: Marca/Create
        public IActionResult Create()
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Marca/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ModelMarca modelMarca)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(modelMarca);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marca creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelMarca);
        }

        // GET: Marca/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca.FindAsync(id);
            if (modelMarca == null)
            {
                return NotFound();
            }
            return View(modelMarca);
        }

        // POST: Marca/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ModelMarca modelMarca)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            if (id != modelMarca.Id)
            {
                TempData["Error"] = "Debe seleccionar una marca.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelMarca);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Marca modificado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelMarcaExists(modelMarca.Id))
                    {
                        TempData["Error"] = "Marca no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelMarca);
        }

        // GET: Marca/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {// Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelMarca = await _context.modelMarca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelMarca == null)
            {
                return NotFound();
            }

            return View(modelMarca);
        }

        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            {
                // Nivel 4 tiene acceso
            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            {
                // Nivel 5 tiene acceso
            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso
            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 2 tiene acceso
            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2.";
                return RedirectToAction("Index", "Home");
            }

            // Buscar la marca que se desea eliminar
            var modelMarca = await _context.modelMarca.FindAsync(id);
            if (modelMarca == null)
            {
                TempData["Error"] = "La marca que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si hay productos asociados a esta marca
            var relatedProducts = await _context.modelProduct.AnyAsync(p => p.IdMarca == id);
            if (relatedProducts)
            {
                TempData["Error"] = "No se puede eliminar la marca porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay productos relacionados, proceder con la eliminación
            try
            {
                _context.modelMarca.Remove(modelMarca);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marca eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar la marca: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ModelMarcaExists(int id)
        {
            return _context.modelMarca.Any(e => e.Id == id);
        }
    }
}
