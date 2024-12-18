using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.Task;


namespace SysSoniaInventory.Controllers
{
    public class ProductController : Controller
    {
        private readonly DBContext _context;

        public ProductController(DBContext context)
        {
            _context = context;
        }

        // GET: Product
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.modelProduct.Include(m => m.IdCategoryNavigation).Include(m => m.IdProveedorNavigation);
            return View(await dBContext.ToListAsync());
        }


        // Acción para buscar productos en tiempo real
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
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


        // GET: Product/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProduct = await _context.modelProduct
                .Include(m => m.IdCategoryNavigation)
                .Include(m => m.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProduct == null)
            {
                return NotFound();
            }

            return View(modelProduct);
        }

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name");
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name");
            return View();
        }


        // POST: Product/Create 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdCategory,IdProveedor,Name,PurchasePrice,SalePrice,Stock,Codigo,Url,Estatus")] ModelProduct modelProduct, string DescriptionCambio, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Si hay una imagen en el campo Url, procesarla
                    if (imagen != null && imagen.Length > 0)
                    {
                        var imgSave = new ImgSave();

                        // Llamar a la función GuardarImagen y obtener la ruta
                        string rutaImagen = imgSave.GuardarImagen(imagen, modelProduct.Id, modelProduct.Name);

                        // Asignar la ruta al campo Url del modelo
                        modelProduct.Url = rutaImagen;
                    }

                    // Agregar el producto a la base de datos
                    _context.Add(modelProduct);
                    await _context.SaveChangesAsync();

                    // Asignar "Sin descripción" si el campo está vacío o es nulo
                    DescriptionCambio ??= "Sin descripción";

                    // Crear el historial del nuevo producto
                    var historial = new ModelHistorialProduct
                    {
                        NameUser = "NPueba",
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
                    // Manejo de errores opcional
                    ModelState.AddModelError("", "Error al crear el producto: " + ex.Message);
                }
            }

            return View(modelProduct);
        }


        // GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProduct = await _context.modelProduct.FindAsync(id);
            if (modelProduct == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.modelCategory, "Id", "Name", modelProduct.IdCategory);
            ViewData["IdProveedor"] = new SelectList(_context.modelProveedor, "Id", "Name", modelProduct.IdProveedor);
            return View(modelProduct);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdCategory,IdProveedor,Name,PurchasePrice,SalePrice,Stock,Codigo,Url,Estatus")] ModelProduct modelProduct, string DescriptionCambio)
        {
            if (id != modelProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el producto original antes de la edición
                    var originalProduct = await _context.modelProduct.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Actualizar el producto en la base de datos
                    _context.Update(modelProduct);
                    await _context.SaveChangesAsync();

                    // Asignar "Sin descripción" si el campo está vacío o es nulo
                    DescriptionCambio ??= "Sin descripción";

                    // Crear el historial con los datos previos y nuevos
                    var historial = new ModelHistorialProduct
                    {
                        NameUser = "NPueba",
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
                        return NotFound();
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







        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelProduct = await _context.modelProduct
                .Include(m => m.IdCategoryNavigation)
                .Include(m => m.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modelProduct == null)
            {
                return NotFound();
            }

            return View(modelProduct);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string DescriptionCambio)
        {
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

                // Eliminar el producto
                _context.modelProduct.Remove(modelProduct);
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
