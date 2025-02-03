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
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        private readonly DBContext _context;

        public RolController(DBContext context)
        {
            _context = context;
        }

       
        // GET: Rol/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            } // Verificar niveles de acceso
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }

            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }
            var roles = _context.modelRol.OrderByDescending(r => r.Id).ToList();
            return View(roles);
        }


        // GET: Rol/Details/5
        public async Task<IActionResult> Details(int? id)
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
            if (id == null)
            {
                TempData["Error"] = "Debe seleccionar un rol.";
                return RedirectToAction(nameof(Index));
            }

            var modelRol = await _context.modelRol
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelRol == null)
            {
                TempData["Error"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(modelRol);
        }

        public async Task<IActionResult> UsuariosPorRol(int id, int page = 1)
        {
            // Verificar si el rol existe
            var rol = await _context.modelRol.FindAsync(id);
            if (rol == null)
            {
                TempData["Error"] = "Debe seleccionar un id de rol válido.";
                return RedirectToAction(nameof(Index));
            }

            int pageSize = 10;  // Número de usuarios por página

            // Obtener la consulta de usuarios con la relación al rol
            var query = _context.modelUser
                .Include(u => u.IdRolNavigation)  // Incluir la relación con el rol
                .Where(u => u.IdRol == id)  // Filtrar por IdRol
                .AsQueryable();

            // Contar el total de usuarios para el rol
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
                TempData["reporte"] = $"No se encontraron usuarios para el rol '{rol.Name}'.";
            }

            // Pasar datos a la vista
            ViewBag.RolId = id;
            ViewBag.RolNombre = rol.Name;  // Pasar el nombre del rol
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);  // Calcular el total de páginas

            return View(usuarios);
        }
        public async Task<IActionResult> DescargarUsuariosPorRol(int id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Verificar si el rol existe
            var rol = await _context.modelRol.FindAsync(id);
            if (rol == null)
            {
                TempData["Error"] = "Debe seleccionar un ID de rol válido.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener los usuarios asociados al rol
            var usuarios = await _context.modelUser
                .Where(u => u.IdRol == id)
                .ToListAsync();

            if (!usuarios.Any())
            {
                TempData["Error"] = $"No se encontraron usuarios para el rol '{rol.Name}'.";
                return RedirectToAction(nameof(UsuariosPorRol), new { id });
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
                titleCell.Add(new Paragraph($"Usuarios del Rol: {rol.Name}")
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
                        .SetBackgroundColor(rowColor));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.LastName))
                        .SetBackgroundColor(rowColor));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.Email))
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
                return File(stream.ToArray(), "application/pdf", $"Usuarios_Rol_{rol.Name.Replace(" ", "_")}_{fechaDescarga}.pdf");
            }
        }


        // GET: Rol/Create
        public IActionResult Create()
        { // Verificar niveles de acceso
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

        // POST: Rol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AccessTipe")] ModelRol modelRol)
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

            // Validar AccessTipe
            var validLevels = new[] { "Nivel 1", "Nivel 2", "Nivel 3", "Nivel 4" };
            if (!validLevels.Contains(modelRol.AccessTipe))
            {
                TempData["Error"] = "Inconsistencia al asignar el tipo de acceso, se restablecio a Nivel 1.";
                modelRol.AccessTipe = "Nivel 1";
            }

            if (ModelState.IsValid)
            {
                _context.Add(modelRol);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Rol creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error al validar uno o más campos.";
            return View(modelRol);
        }

        // GET: Rol/Edit/5
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
                TempData["Error"] = "Debe seleccionar un rol.";
                return RedirectToAction(nameof(Index));
            }

            var modelRol = await _context.modelRol.FindAsync(id);
            if (modelRol == null)
            {
                TempData["Error"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(modelRol);
        }

        // POST: Rol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AccessTipe")] ModelRol modelRol)
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
            if (id != modelRol.Id)
            {
                TempData["Error"] = "El id del rol no coincide.";
                return RedirectToAction(nameof(Index));
            }




            // Validar AccessTipe
            var validLevels = new[] { "Nivel 1", "Nivel 2", "Nivel 3", "Nivel 4" };
            if (!validLevels.Contains(modelRol.AccessTipe))
            {
                TempData["Error"] = "Inconsistencia al asignar el tipo de acceso, se restablecio a Nivel 1.";
                modelRol.AccessTipe = "Nivel 1";
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelRol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelRolExists(modelRol.Id))
                    {
                        TempData["Error"] = "Rol no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Rol modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Error inesperado en la validación de un campo o más.";
            return View(modelRol);
        }

        // GET: Rol/Delete/5
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
                TempData["Error"] = "Debe seleccionar un rol.";
                return RedirectToAction(nameof(Index));
            }

            var modelRol = await _context.modelRol
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelRol == null)
            {
                TempData["Error"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(modelRol);
        }

        // POST: Rol/Delete/5
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

            var modelRol = await _context.modelRol.FindAsync(id);

            if (modelRol == null)
            {
                TempData["Error"] = "El rol que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar relaciones activas en ModelUser
            var relatedUsers = await _context.modelUser.AnyAsync(u => u.IdRol == id);
            if (relatedUsers)
            {
                TempData["Error"] = "No se puede eliminar el rol porque tiene usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }

            // Si no hay relaciones, eliminar el rol
            try
            {
                _context.modelRol.Remove(modelRol);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Rol eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el rol: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }



        private bool ModelRolExists(int id)
        {
            return _context.modelRol.Any(e => e.Id == id);
        }
    }
}
