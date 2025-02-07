using System;
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
using Microsoft.IdentityModel.Tokens;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class SucursalController : Controller
    {
        private readonly DBContext _context;

        public SucursalController(DBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchName, int page = 1)
        { // Verificar niveles de acceso
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
            int pageSize = 5; // Cantidad de roles por página
            var query = _context.modelSucursal.AsQueryable();

            // Aplicar filtros si se proporcionan
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(r => r.Name.Contains(searchName));
            }

            // Obtener total de registros antes de paginar
            int totalSucursal= await query.CountAsync();

            // Ordenar y paginar resultados
            var sucursales = await query
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Datos para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);
            ViewBag.SearchName = searchName;
           

            return View(sucursales);
        }

        // GET: Sucursal/Details/5
        public async Task<IActionResult> Details(int? id)
        {// Verificar niveles de acceso
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

            var modelSucursal = await _context.modelSucursal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelSucursal == null)
            {
                return NotFound();
            }

            return View(modelSucursal);
        }

        public async Task<IActionResult> UsuariosPorSucursal(int id, int page = 1)
        {
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
            // Verificar si la sucursal existe
            var sucursal = await _context.modelSucursal.FindAsync(id);
            if (sucursal == null)
            {
                TempData["Error"] = "Debe seleccionar un id de sucursal válida.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de usuarios por página

            // Obtener la consulta de usuarios con la relación a la sucursal
            var query = _context.modelUser
                .Include(u => u.IdSucursalNavigation)  // Incluir la relación con la sucursal
                .Where(u => u.IdSucursal == id)  // Filtrar por IdSucursal
                .AsQueryable();

            // Contar el total de usuarios para la sucursal
            int totalUsuarios = await query.CountAsync();

            // Aplicar paginación
            var usuarios = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Verificar si no se encontraron usuarios
            if (!usuarios.Any())
            {
                TempData["reporte"] = $"No se encontraron usuarios para la sucursal '{sucursal.Name}'.";
            }

            // Pasar datos a la vista
            ViewBag.SucursalId = id;
            ViewBag.SucursalNombre = sucursal.Name;  // Pasar el nombre de la sucursal
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);  // Calcular el total de páginas

            return View(usuarios);
        }

 
        // GET: Sucursal/Create
        public IActionResult Create()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }


            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Sucursal/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] ModelSucursal modelSucursal)
        {
             if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(modelSucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sucursal creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelSucursal);
        }

        // GET: Sucursal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelSucursal = await _context.modelSucursal.FindAsync(id);
            if (modelSucursal == null)
            {
                return NotFound();
            }
            return View(modelSucursal);
        }

        // POST: Sucursal/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] ModelSucursal modelSucursal)
        {
            // Verificar niveles de acceso
          if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            if (id != modelSucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelSucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelSucursalExists(modelSucursal.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Sucursal modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelSucursal);
        }

        // GET: Sucursal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
          if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }


            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var modelSucursal = await _context.modelSucursal
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelSucursal == null)
            {
                return NotFound();
            }

            return View(modelSucursal);
        }

        // POST: Sucursal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar si el usuario tiene el nivel de acceso requerido
            if (!User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 5.";
                return RedirectToAction("Index", "Home");
            }

            var modelSucursal = await _context.modelSucursal.FindAsync(id);

            if (modelSucursal == null)
            {
                TempData["Error"] = "La sucursal que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si hay usuarios asociados a esta sucursal
            var relatedUsers = await _context.modelUser.AnyAsync(u => u.IdSucursal == id);
            if (relatedUsers)
            {
                TempData["Error"] = "No se puede eliminar la sucursal porque tiene usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay relaciones, eliminar la sucursal
            try
            {
                _context.modelSucursal.Remove(modelSucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sucursal eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar la sucursal: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        // Método para generar PDF de todas las sucursales
        public IActionResult GeneratePdf()
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener la lista de sucursales
            var sucursales = _context.Set<ModelSucursal>().ToList();

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Agregar logo y título en la misma línea
                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                Table headerTable = new Table(new float[] { 1, 3 }).UseAllAvailableWidth();

                if (System.IO.File.Exists(imagePath))
                {
                    var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(80, 80);
                    Cell logoCell = new Cell().Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    // Celda vacía si no hay logo
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }


                // Celda del título
                Cell titleCell = new Cell().Add(new Paragraph("Todas las Sucursales")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                // Crear tabla
                var table = new Table(new float[] { 1, 2, 3 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados estilizados
                var headerColor = new DeviceRgb(52, 152, 219); // Azul intenso
                foreach (var header in new[] { "ID", "Nombre", "Dirección" })
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
                foreach (var sucursal in sucursales)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(sucursal.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(sucursal.Name))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(sucursal.Address ?? "N/A"))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
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
                // Retornar archivo PDF
                return File(stream.ToArray(), "application/pdf", $"Sucursales_{fechaDescarga}.pdf");
            }
        }


        public async Task<IActionResult> DescargarUsuariosPorSucursal(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar si la sucursal existe
            var sucursal = await _context.modelSucursal.FindAsync(id);
            if (sucursal == null)
            {
                TempData["Error"] = "Debe seleccionar un ID de sucursal válido.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los usuarios asociados a la sucursal
            var usuarios = await _context.modelUser
                .Where(u => u.IdSucursal == id)
                .ToListAsync();

            if (!usuarios.Any())
            {
                TempData["Error"] = $"No se encontraron usuarios para la sucursal '{sucursal.Name}'.";
                return RedirectToAction(nameof(UsuariosPorSucursal), new { id });
            }

            // Generar el PDF
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Cabecera con el logo y el título
                var headerTable = new Table(new float[] { 1, 4 }).SetWidth(UnitValue.CreatePercentValue(100));
                headerTable.SetBorder(Border.NO_BORDER);

                // Logo
                var logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgSystem", "LOGO.jpeg");
                var logo = new Image(ImageDataFactory.Create(imagePath)).ScaleAbsolute(100, 100).SetMarginTop(-50);
                logoCell.Add(logo);

                // Título
                var titleCell = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                titleCell.Add(new Paragraph($"Usuarios de la Sucursal: {sucursal.Name}")
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetMarginBottom(10));

                headerTable.AddCell(logoCell);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                // Tabla de usuarios
                var table = new Table(new float[] { 1, 3, 3, 4 }).SetWidth(UnitValue.CreatePercentValue(100));
                var headerColor = new DeviceRgb(41, 128, 185);
                foreach (var header in new[] { "ID", "Nombre", "Apellido", "Email" })
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
                foreach (var usuario in usuarios)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;

                    table.AddCell(new Cell().Add(new Paragraph(usuario.Id.ToString()))
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.Name))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.LastName))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.Email))
                        .SetBackgroundColor(rowColor)
                        .SetTextAlignment(TextAlignment.CENTER).SetTextAlignment(TextAlignment.CENTER));

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
                return File(stream.ToArray(), "application/pdf", $"Usuarios_Sucursal_{sucursal.Name.Replace(" ", "_")}_{fechaDescarga}.pdf");
            }
        }



        private bool ModelSucursalExists(int id)
        {
            return _context.modelSucursal.Any(e => e.Id == id);
        }
    }
}
