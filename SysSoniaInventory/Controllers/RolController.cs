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
        public async Task<IActionResult> Index(string searchName, string searchType, int page = 1)
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
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4.";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 5; // Cantidad de roles por página
            var query = _context.modelRol.AsQueryable();

            // Excluir registros con AccessTipe "Nivel 5"
            query = query.Where(r => r.AccessTipe != "Nivel 5");

            // Aplicar filtros si se proporcionan
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(r => r.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                query = query.Where(r => r.AccessTipe.Contains(searchType));
            }

            // Obtener total de registros antes de paginar
            int totalRoles = await query.CountAsync();

            // Ordenar y paginar resultados
            var roles = await query
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Datos para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize);
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;

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

        // Método para generar PDF  
        public IActionResult GeneratePdf(bool? active = null)
        {
            // Verificar niveles de acceso  
            if (!User.HasClaim("AccessTipe", "Nivel 4") && !User.HasClaim("AccessTipe", "Nivel 5"))
            {
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 4 o 5.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener datos de roles (filtrar por estado si se especifica)  
            var roles = _context.modelRol.AsQueryable();

            if (active.HasValue)
            {
                roles = roles.Where(r => r.User.Any(u => u.Estatus == (active.Value ? 1 : 0)));
            }

            var roleList = roles.ToList();

            // Generar PDF  
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
                Cell titleCell = new Cell().Add(new Paragraph("Todos los Roles")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.LEFT))
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(titleCell);
                document.Add(headerTable);

                // Crear tabla  
                var table = new Table(new float[] { 1, 2, 2 }).SetWidth(UnitValue.CreatePercentValue(100));
                table.SetMarginTop(10);

                // Encabezados estilizados  
                var headerColor = new DeviceRgb(52, 152, 219); // Azul intenso  
                foreach (var header in new[] { "ID", "Nombre", "Tipo de Acceso" })
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
                foreach (var role in roleList)
                {
                    var rowColor = isAlternate ? alternateRowColor : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(role.Id.ToString()))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(role.Name))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(role.AccessTipe))
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
                return File(stream.ToArray(), "application/pdf", $"Roles_{fechaDescarga}.pdf");
            }
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
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell().Add(new Paragraph(usuario.LastName))
                        .SetBackgroundColor(rowColor).SetTextAlignment(TextAlignment.CENTER));

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



        private bool ModelRolExists(int id)
        {
            return _context.modelRol.Any(e => e.Id == id);
        }
    }
}
