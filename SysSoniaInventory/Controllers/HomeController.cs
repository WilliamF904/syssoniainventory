using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.Task;
using SysSoniaInventory.ViewModels;
using System.Diagnostics;

namespace SysSoniaInventory.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }


        public async Task<IActionResult> IndexAsync()
        {
            // Verificar niveles de acceso
            if (User.HasClaim("AccessTipe", "Nivel 5"))
            { // Nivel 4 tiene acceso
                var facturas = _context.modelFactura
                .Include(f => f.DetalleFactura)
                .Select(f => new FacturaViewModel
                {
                    Id = f.Id,
                    NameUser = f.NameUser,
                    Date = f.Date,
                    Time = f.Time,
                    TotalFactura = f.DetalleFactura.Sum(d => d.PriceTotal),
                    Detalles = f.DetalleFactura.Select(d => new DetalleFacturaViewModel
                    {
                        CodigoProducto = d.CodigoProducto,
                        CantidadProduct = d.CantidadProduct
                    }).ToList()
                })
                .ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                return View(facturas);
            }
            if (User.HasClaim("AccessTipe", "Nivel 4"))
            { // Nivel 4 tiene acceso
                var facturas = _context.modelFactura
                .Include(f => f.DetalleFactura)
                .Select(f => new FacturaViewModel
                {
                    Id = f.Id,
                    NameUser = f.NameUser,
                    Date = f.Date,
                    Time = f.Time,
                    TotalFactura = f.DetalleFactura.Sum(d => d.PriceTotal),
                    Detalles = f.DetalleFactura.Select(d => new DetalleFacturaViewModel
                    {
                        CodigoProducto = d.CodigoProducto,
                        CantidadProduct = d.CantidadProduct
                    }).ToList()
                })
                .ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                return View(facturas);
            }
            else if (User.HasClaim("AccessTipe", "Nivel 3"))
            {
                // Nivel 3 tiene acceso
                var facturas = _context.modelFactura
                .Include(f => f.DetalleFactura)
                .Select(f => new FacturaViewModel
                {
                    Id = f.Id,
                    NameUser = f.NameUser,
                    Date = f.Date,
                    Time = f.Time,
                    TotalFactura = f.DetalleFactura.Sum(d => d.PriceTotal),
                    Detalles = f.DetalleFactura.Select(d => new DetalleFacturaViewModel
                    {
                        CodigoProducto = d.CodigoProducto,
                        CantidadProduct = d.CantidadProduct
                    }).ToList()
                })
                .ToList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                return View(facturas);

            }
            else if (User.HasClaim("AccessTipe", "Nivel 2"))
            {
                // Nivel 2 tiene acceso

            }
            else if (User.HasClaim("AccessTipe", "Nivel 1"))
            {
                // Nivel 1 tiene acceso

            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // Redirigir con mensaje de error si el usuario no tiene acceso
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 1 o superior.";

                return RedirectToAction("Login", "Auth");
            }


            return View();

        }










        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }










    }
}
