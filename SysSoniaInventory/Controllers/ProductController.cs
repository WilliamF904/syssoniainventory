using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.Task;
using System.IO;
using iText.Commons.Actions.Contexts;
using SysSoniaInventory.ViewModels;


namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly DBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        [HttpGet]
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
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 2 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }

            var dBContext = _context.modelProduct.Include(m => m.IdCategoryNavigation).Include(m => m.IdProveedorNavigation);
            return View(await dBContext.OrderByDescending(r => r.Id).ToListAsync());
        }


        // Acción para buscar productos en tiempo real
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
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
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(term))
            {
                return Json(new { results = new List<object>() });
            }

            var products = await _context.modelProduct
                .Where(p => EF.Functions.Like(p.Name, $"%{term}%"))
                .Select(p => new { p.Id, p.Name })
                .Take(10)
                .ToListAsync();

            return Json(new { results = products });
        }




    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
                 // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5") ||
                User.HasClaim("AccessTipe", "Nivel 4") ||
                User.HasClaim("AccessTipe", "Nivel 3"))
            {
               
            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Redirigir a Detail, Product
                return RedirectToAction("Detail", "Product", new { id = id });
            }
            else 
            {
               // Manejo por defecto si no hay nivel definido
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 2 o superior.";
                return RedirectToAction("Index", "Home");
            }


            if (id == null)
        {
            TempData["Error"] = "Debes seleccionar un producto.";
            return RedirectToAction(nameof(Index));
        }

        var modelProduct = await _context.modelProduct
            .Include(m => m.IdCategoryNavigation)
            .Include(m => m.IdProveedorNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (modelProduct == null)
        {
            TempData["Error"] = "Producto no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        // Verificar si hay reportes pendientes para el producto del tipo "Stock Bajo"
        var reportePendiente = await _context.modelReport
            .FirstOrDefaultAsync(r => r.IdRelation == id && r.TypeReport == "Stock Bajo" && r.Estatus != "Finalizado");

        if (reportePendiente != null)
        {
            ViewBag.CasoPendiente = "Reporte de este producto: Stock bajo; Estado: Pendiente.";
            ViewBag.ReporteId = reportePendiente.Id; // Pasar el ID del reporte
        }

        // Calcular las ganancias mensuales para los últimos 12 meses
        var detallesFactura = await _context.modelDetalleFactura
            .Include(d => d.IdFacturaNavigation)
            .Where(d => d.IdProduct == id &&
                        d.IdFacturaNavigation.Date >= DateOnly.FromDateTime(DateTime.Now.AddMonths(-12)))
            .ToListAsync();

        var gananciasPorMes = detallesFactura
            .GroupBy(d => new { d.IdFacturaNavigation.Date.Year, d.IdFacturaNavigation.Date.Month })
            .Select(g => new ProductoViewModel.GananciaMensualViewModel
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                Fecha = $"{g.Key.Year}-{g.Key.Month:00}",
                Ganancia = g.Sum(d => (d.SalePriceDescuento - d.PurchasePriceUnitario) * d.CantidadProduct),
                Cantidad = g.Sum(d => d.CantidadProduct)
            })
            .OrderByDescending(g => g.Fecha) // Más reciente primero
            .ToList();

        ViewBag.GananciasPorMes = gananciasPorMes;

        return View(modelProduct);
    }




        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            // Verificar niveles de acceso
            if (!User.HasClaim("AccessTipe", "Nivel 2"))
            {
                return RedirectToAction("Details", "Product", new { id = id });
            }
        
            if (id == null)
            {
                TempData["Error"] = "Debes seleccionar un producto.";
                return RedirectToAction(nameof(Index));
            }

            var modelProduct = await _context.modelProduct
                .Include(m => m.IdCategoryNavigation)
                .Include(m => m.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modelProduct == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(modelProduct);
        }

        // GET: Product/Create
        [HttpGet]
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
            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name");
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name");
            return View();
        }


        // POST: Product/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdCategory,IdProveedor,Name,PurchasePrice,SalePrice,Stock,LowStock,Codigo,Url,Estatus")] ModelProduct modelProduct, string DescriptionCambio, IFormFile imagen)
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

            if (modelProduct.LowStock == null || modelProduct.LowStock == 0)
            {
                modelProduct.LowStock = 10;
            }
            ModelState.Remove("imagen");
            ModelState.Remove("LowStock");
            if (ModelState.IsValid)
            {
                try
                {
                    // Agregar el producto inicial a la base de datos sin la URL de la imagen
                    _context.Add(modelProduct);
                    TempData["Success"] = "Producto creado correctamente.";
                    await _context.SaveChangesAsync();

                    // Si hay una imagen, procesarla usando el ID generado
                    if (imagen != null && imagen.Length > 0)
                    {
                        var imgSave = new ImgSave();

                        // Llamar a la función GuardarImagen y obtener la ruta
                        string rutaImagen = imgSave.GuardarImagen(imagen, modelProduct.Id, modelProduct.Name);

                        // Actualizar el producto con la URL de la imagen
                        modelProduct.Url = rutaImagen;
                        _context.Update(modelProduct);
                        await _context.SaveChangesAsync();
                    }

                    // Asignar "Sin descripción" si el campo está vacío o es nulo
                    DescriptionCambio ??= "Sin descripción";

                    // Crear el historial del nuevo producto
                    var historial = new ModelHistorialProduct
                    {
                        NameUser = User.Identity?.Name,
                        IdProduct = modelProduct.Id,
                        AfterNameProduct = modelProduct.Name,
                        AfterPurchasePrice = modelProduct.PurchasePrice,
                        AfterSalePrice = modelProduct.SalePrice,
                        AfterStock = modelProduct.Stock,
                        AfterCodigo = modelProduct.Codigo,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Time = TimeOnly.FromDateTime(DateTime.Now),
                        RazonCambioAuto = "Nuevo producto",
                        DescriptionCambio = DescriptionCambio
                    };

                    // Agregar el historial a la base de datos
                    _context.Add(historial);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                   
                    TempData["Error"] = "Error al intentar crear el producto.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(modelProduct);
        }

        // GET: Product/Edit/5
        [HttpGet]
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
                TempData["Error"] = "Debese seleccionar un producto.";
                return RedirectToAction(nameof(Index));
            }

            var modelProduct = await _context.modelProduct.FindAsync(id);
            if (modelProduct == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            // Verificar si hay reportes pendientes para el producto del tipo "Stock Bajo"
            var reportesPendientes = await _context.modelReport
                .Where(r => r.IdRelation == id && r.TypeReport == "Stock Bajo" && r.Estatus != "Finalizado")
                .ToListAsync();

            if (reportesPendientes.Any())
            {
                // Enviar mensaje a la vista si hay reportes pendientes de tipo "Stock Bajo"
                ViewBag.CasoPendiente = "Reporte de este producto: Stock bajo; Estado: Pendiente.";
            }
            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name", modelProduct.IdCategory);
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name", modelProduct.IdProveedor);
            return View(modelProduct);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdCategory,IdProveedor,Name,PurchasePrice,SalePrice,Stock,LowStock,Codigo,Url,Estatus")] ModelProduct modelProduct, string DescriptionCambio, IFormFile imagen)
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
            if (id != modelProduct.Id)
            {
                TempData["Error"] = "El id del producto debe ser el mismo al editar.";
                return RedirectToAction(nameof(Index));
            }

            if (modelProduct.LowStock == null || modelProduct.LowStock == 0)
            {
                modelProduct.LowStock = 10;
            }
            ModelState.Remove("imagen");
            ModelState.Remove("LowStock");
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el producto original antes de la edición
                    var originalProduct = await _context.modelProduct.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    modelProduct.Stock = originalProduct.Stock;
                    if (originalProduct == null)
                    {
                        return NotFound();
                    }

                    // Si se proporciona una nueva imagen, procesarla
                    if (imagen != null && imagen.Length > 0)
                    {
                        //Aqui valida la ruta desde el campo del modelo guardado en la base
                        var imgDelete = new ImgDelete();

                        // Eliminar la imagen anterior si existe
                        if (!string.IsNullOrEmpty(originalProduct.Url))
                        {
                            imgDelete.EliminarImagenPorUrl(originalProduct.Url);
                        }

                        var imgSave = new ImgSave();
                        string nuevaRutaImagen = imgSave.GuardarImagen(imagen, modelProduct.Id, modelProduct.Name);

                        // Actualizar la URL de la imagen en el producto
                        modelProduct.Url = nuevaRutaImagen;
                    }
                    else
                    {
                        // Mantener la URL de la imagen existente
                        modelProduct.Url = originalProduct.Url;
                    }

                    // Actualizar el producto en la base de datos
                    _context.Update(modelProduct);
                    TempData["Success"] = "Producto editado correctamente.";  
                    await _context.SaveChangesAsync();



                    // Asignar "Sin descripción" si el campo está vacío o es nulo
                    DescriptionCambio ??= "Sin descripción";

                    // Crear el historial con los datos previos y nuevos
                    var historial = new ModelHistorialProduct
                    {
                        NameUser = User.Identity?.Name,
                        IdProduct = modelProduct.Id,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Time = TimeOnly.FromDateTime(DateTime.Now),

                        // Diferenciar el "antes" de la edición de los nuevos datos
                        BeforeNameProduct = originalProduct?.Name,
                        AfterNameProduct = originalProduct?.Name != modelProduct.Name ? modelProduct.Name : null,
                        BeforePurchasePrice = originalProduct?.PurchasePrice,
                        AfterPurchasePrice = originalProduct?.PurchasePrice != modelProduct.PurchasePrice ? modelProduct.PurchasePrice : null,
                        BeforeSalePrice = originalProduct?.SalePrice,
                        AfterSalePrice = originalProduct?.SalePrice != modelProduct.SalePrice ? modelProduct.SalePrice : null,
                        BeforeStock = originalProduct?.Stock,
                        AfterStock = originalProduct?.Stock != modelProduct.Stock ? modelProduct.Stock : null,
                        BeforeCodigo = originalProduct?.Codigo,
                        AfterCodigo = originalProduct?.Codigo != modelProduct.Codigo ? modelProduct.Codigo : null,

                        RazonCambioAuto = "Edición de producto",
                        DescriptionCambio = DescriptionCambio
                    };

                    // Solo agregar historial si hay al menos un cambio
                    if (historial.AfterNameProduct != null ||
                        historial.AfterPurchasePrice != null ||
                        historial.AfterSalePrice != null ||
                        historial.AfterStock != null ||
                        historial.AfterCodigo != null)
                    {
                        _context.Add(historial);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelProductExists(modelProduct.Id))
                    {
                    
                        TempData["Error"] = "Producto no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name", modelProduct.IdCategory);
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name", modelProduct.IdProveedor);
            return View(modelProduct);
        }




        // GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Stock(int? id)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
              
                TempData["Error"] = "Debese seleccionar un producto.";

                return RedirectToAction(nameof(Index));
            }

            var modelProduct = await _context.modelProduct.FindAsync(id);
            if (modelProduct == null)
            {
               
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name", modelProduct.IdCategory);
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name", modelProduct.IdProveedor);


            return View(modelProduct);
        }

        // POST: Product/Stock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Stock(int id, string DescriptionCambio, int modificarStock, bool accionStock)
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 5 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            { // Nivel 5 tiene acceso

            }
            else
            {
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 3.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener el producto desde la base de datos
            var originalProduct = await _context.modelProduct.FirstOrDefaultAsync(p => p.Id == id);
            // Validaciones iniciales
            if (modificarStock <= 0 || modificarStock > 999)
            {
                TempData["Error"] = "El valor de modificación debe estar entre 1 y 999.";
                return RedirectToAction(nameof(Index));
            }


            if (modificarStock > 0 && !accionStock && modificarStock > originalProduct.Stock)
            {
                TempData["Error"] = "No se puede remover más de lo que hay registrado en stock";
                return RedirectToAction(nameof(Index));
            }

            try
            {
               

                if (originalProduct == null)
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Almacenar valores originales para el historial
                var stockAnterior = originalProduct.Stock;

                // Actualizar stock según la acción
                if (accionStock == true)
                {
                    originalProduct.Stock += modificarStock;
                }
                else
                {
                    originalProduct.Stock -= modificarStock;
                }

                // Validar que el stock no sea negativo
                if (originalProduct.Stock < 0)
                {
                    TempData["Error"] = "La operación resultaría en un stock negativo.";
                    return RedirectToAction(nameof(Index));
                }

                // Guardar cambios en la base de datos
                _context.Update(originalProduct);
                await _context.SaveChangesAsync();

                // Registrar historial del cambio
                var historial = new ModelHistorialProduct
                {
                    NameUser = User.Identity?.Name,
                    IdProduct = originalProduct.Id,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Time = TimeOnly.FromDateTime(DateTime.Now),
                    BeforeStock = stockAnterior,
                    AfterStock = originalProduct.Stock,
                    RazonCambioAuto = "Actualización de stock",
                    DescriptionCambio = string.IsNullOrWhiteSpace(DescriptionCambio) ? "Sin descripción" : DescriptionCambio
                };

                _context.Add(historial);


                // Validar si el stock supera LowStock y hay reportes pendientes
                if (accionStock && originalProduct.Stock > originalProduct.LowStock)
                {
                    var reportesPendientes = await _context.modelReport
                        .Where(r => r.IdRelation == id && r.TypeReport == "Stock Bajo" && r.Estatus != "Finalizado")
                        .ToListAsync();

                    foreach (var reporte in reportesPendientes)
                    {
                        reporte.Estatus = "Finalizado";
                        reporte.NameUser = User.Identity?.Name;
                        reporte.ComentaryUser = $"Descripción automática: Se agregó {modificarStock} al stock del producto.";
                        reporte.EndDate = DateOnly.FromDateTime(DateTime.Now);
                        reporte.EndTime = TimeOnly.FromDateTime(DateTime.Now);

                        _context.Update(reporte);
                        TempData["reporte"] = "Reporte: Stock bajo; Estado: Finalizado.";
                    }
                }


                await _context.SaveChangesAsync();



                TempData["Success"] = accionStock ? "Stock agregado correctamente." : "Stock removido correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelProductExists(id))
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
        }








        // GET: Product/Delete/5
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
                TempData["Error"] = "Debese seleccionar un producto.";
                return RedirectToAction(nameof(Index));
            }

            var modelProduct = await _context.modelProduct
                .Include(m => m.IdCategoryNavigation)
                .Include(m => m.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProduct == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(modelProduct);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string DescriptionCambio)
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
            var modelProduct = await _context.modelProduct.FindAsync(id);
            if (modelProduct != null)
            {
                // Crear un registro en el historial
                var historial = new ModelHistorialProduct
                {
                    NameUser = "NPueba",
                    IdProduct = modelProduct.Id,
                    BeforeNameProduct = modelProduct.Name,
                    AfterNameProduct = null,
                    BeforePurchasePrice = modelProduct.PurchasePrice,
                    AfterPurchasePrice = null,
                    BeforeSalePrice = modelProduct.SalePrice,
                    AfterSalePrice = null,
                    BeforeStock = modelProduct.Stock,
                    AfterStock = null,
                    BeforeCodigo = modelProduct.Codigo,
                    AfterCodigo = null,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Time = TimeOnly.FromDateTime(DateTime.Now),
                    RazonCambioAuto = "Eliminación de producto",
                    DescriptionCambio = DescriptionCambio
                };

                _context.Add(historial);

                //Aqui valida la ruta desde el campo del modelo guardado en la base
                var imgDelete = new ImgDelete();

                // Eliminar la imagen anterior si existe
                if (!string.IsNullOrEmpty(modelProduct.Url))
                {
                    imgDelete.EliminarImagenPorUrl(modelProduct.Url);
                }


                // Eliminar el producto
                _context.modelProduct.Remove(modelProduct);
                TempData["Success"] = "Producto eliminado correctamente.";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ModelProductExists(int id)
        {
            return _context.modelProduct.Any(e => e.Id == id);
        }
    }
}
