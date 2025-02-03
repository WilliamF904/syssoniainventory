﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class ProveedorController : Controller
    {
        private readonly DBContext _context;

        public ProveedorController(DBContext context)
        {
            _context = context;
        }

        // GET: Proveedor
        public async Task<IActionResult> Index()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }
            return View(await _context.modelProveedor.OrderByDescending(r => r.Id).ToListAsync());
        }

        // GET: Proveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProveedor == null)
            {
                return NotFound();
            }

            return View(modelProveedor);
        }

        public async Task<IActionResult> ProductosPorProveedor(int id, int page = 1)
        {  // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3 o superior.";
                return RedirectToAction("Index", "Home");
            }
            // Verificar si el proveedor existe
            var proveedor = await _context.modelProveedor.FindAsync(id);
            if (proveedor == null)
            {
                TempData["Error"] = "Debe seleccionar un id de proveedor valido.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de productos por página

            // Obtener la consulta de productos con las relaciones necesarias
            var query = _context.modelProduct
                .Include(p => p.IdCategoryNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Where(p => p.IdProveedor == id) // Filtrar por IdProveedor
                .AsQueryable();

            // Contar el total de productos para el proveedor
            int totalProductos = await query.CountAsync();

            // Aplicar paginación
            var productos = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (!productos.Any())
            {
                TempData["reporte"] = $"No se encontraron productos relacionados con el proveedor '{proveedor.Name}'.";
            }
            // Pasar datos a la vista
            ViewBag.ProveedorId = id;
            ViewBag.ProveedorNombre = proveedor.Name; // Pasar el nombre del proveedor
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProductos / pageSize); // Calcular el total de páginas

            return View(productos);
        }
        public async Task<IActionResult> DescargarProductosPorProveedor(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar si el proveedor existe
            var proveedor = await _context.modelProveedor.FindAsync(id);
            if (proveedor == null)
            {
                TempData["Error"] = "Debe seleccionar un ID de proveedor válido.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los productos asociados al proveedor
            var productos = await _context.modelProduct
                .Where(p => p.IdProveedor == id)
                .ToListAsync();

            if (!productos.Any())
            {
                TempData["Error"] = $"No se encontraron productos relacionados con el proveedor '{proveedor.Name}'.";
                return RedirectToAction(nameof(ProductosPorProveedor), new { id });
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
                titleCell.Add(new Paragraph($"Productos del Proveedor: {proveedor.Name}")
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
                return File(stream.ToArray(), "application/pdf", $"Productos_Proveedor_{proveedor.Name.Replace(" ", "_")}_{fechaDescarga}.pdf");
            }
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Proveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Tel,Email")] ModelProveedor modelProveedor)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }


            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(modelProveedor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelProveedor);
        }

        // GET: Proveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor.FindAsync(id);
            if (modelProveedor == null)
            {
                return NotFound();
            }
            return View(modelProveedor);
        }

        // POST: Proveedor/Edit/5
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Tel,Email")] ModelProveedor modelProveedor)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            if (id != modelProveedor.Id)
            {
                TempData["Error"] = "Debe seleccionar un proveedor.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelProveedor);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Proveedor modificado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelProveedorExists(modelProveedor.Id))
                    {
                        TempData["Error"] = "Proveedor no encontrado.";
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
            return View(modelProveedor);
        }

        // GET: Proveedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelProveedor = await _context.modelProveedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProveedor == null)
            {
                return NotFound();
            }

            return View(modelProveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") || !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            var modelProveedor = await _context.modelProveedor.FindAsync(id);

            if (modelProveedor == null)
            {
                TempData["Error"] = "El proveedor que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si hay productos asociados a este proveedor
            var relatedProducts = await _context.modelProduct.AnyAsync(p => p.IdProveedor == id);
            if (relatedProducts)
            {
                TempData["Error"] = "No se puede eliminar el proveedor porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay productos relacionados, eliminar el proveedor
            try
            {
                _context.modelProveedor.Remove(modelProveedor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el proveedor: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ModelProveedorExists(int id)
        {
            return _context.modelProveedor.Any(e => e.Id == id);
        }
    }
}
