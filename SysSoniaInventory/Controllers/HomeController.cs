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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var userName = User.Identity.Name;




            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4"))
            {

                var reportesNoFinalizados = await _context.modelReport
    .Where(r => r.Estatus != "Finalizado")
    .OrderBy(r => r.StarDate).ThenBy(r => r.StarTime)
    .Take(5)
    .ToListAsync();

            var reportesFinalizados = await _context.modelReport
                .Where(r => r.Estatus == "Finalizado")
                .OrderBy(r => r.StarDate).ThenBy(r => r.StarTime)
                .Take(5 - reportesNoFinalizados.Count)
                .ToListAsync();
         
                var reportesCombinados = reportesNoFinalizados.Concat(reportesFinalizados).ToList();

                ViewBag.Reportes = reportesCombinados;
                ViewBag.MostrarBotonVerTodos = reportesNoFinalizados.Count > 5;

            }











            var facturas = _context.modelFactura
                .Include(f => f.DetalleFactura)
                .Where(f => f.Date.Year == currentYear && f.Date.Month == currentMonth)
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
                        NameProduct = d.NameProducto,
                        CantidadProduct = d.CantidadProduct
                    }).ToList()
                })
                .ToList();

            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4") || User.HasClaim("AccessTipe", "Nivel 3"))
            {
                var ventasPorUsuario = facturas
                    .GroupBy(f => f.NameUser)
                    .Select(g => new UsuariosVentasViewModel
                    {
                        NameUser = g.Key,
                        TotalFacturas = g.Sum(f => f.TotalFactura)
                    })
                    .OrderByDescending(u => u.TotalFacturas)
                    .ToList();

                int totalUsuarios = ventasPorUsuario.Count;

                var usuariosPaginados = ventasPorUsuario
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var usuariosConPosicion = usuariosPaginados.Select((usuario, index) => new
                {
                    Index = index + 1,
                    usuario.NameUser,
                    usuario.TotalFacturas
                }).ToList();

                var productosGrafico = facturas
                    .SelectMany(f => f.Detalles)
                    .GroupBy(d => d.CodigoProducto)
                    .Select(g => new
                    {
                        CodigoProducto = g.Key,
                        NameProduct = g.First().NameProduct,
                        CantidadVendida = g.Sum(d => d.CantidadProduct)
                    })
                    .OrderByDescending(p => p.CantidadVendida)
                    .Take(10)
                    .ToList();

                var facturasUsuario = facturas.Where(f => f.NameUser == userName).ToList();
                var totalVentasUsuario = facturasUsuario.Sum(f => f.TotalFactura);

                var productosVendidosUsuario = facturasUsuario
                    .SelectMany(f => f.Detalles)
                    .GroupBy(d => d.CodigoProducto)
                    .Select(g => new
                    {
                        CodigoProducto = g.Key,
                        NameProduct = g.First().NameProduct,
                        CantidadVendida = g.Sum(d => d.CantidadProduct)
                    })
                    .OrderByDescending(p => p.CantidadVendida)
                    .Take(10)
                    .ToList();

                ViewBag.Page = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalUsuarios / (double)pageSize);
                ViewBag.VentasPorUsuario = usuariosConPosicion;
                ViewBag.ProductosGrafico = productosGrafico;
                ViewBag.UserName = userName;
                ViewBag.TotalVentasUsuario = totalVentasUsuario;
                ViewBag.ProductosVendidosUsuario = productosVendidosUsuario;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                return View(facturas);
            }
            else if (User.HasClaim("AccessTipe", "Nivel 2") || User.HasClaim("AccessTipe", "Nivel 1"))
            {
                var facturasUsuario = facturas.Where(f => f.NameUser == userName).ToList();
                var totalVentasUsuario = facturasUsuario.Sum(f => f.TotalFactura);

                var productosVendidosUsuario = facturasUsuario
                    .SelectMany(f => f.Detalles)
                    .GroupBy(d => d.CodigoProducto)
                    .Select(g => new
                    {
                        CodigoProducto = g.Key,
                        NameProduct = g.First().NameProduct,
                        CantidadVendida = g.Sum(d => d.CantidadProduct)
                    })
                    .OrderByDescending(p => p.CantidadVendida)
                    .Take(10)
                    .ToList();

                ViewBag.Page = page;
                ViewBag.TotalPages = 0;
                ViewBag.VentasPorUsuario = null;
                ViewBag.ProductosGrafico = null;
                ViewBag.UserName = userName;
                ViewBag.TotalVentasUsuario = totalVentasUsuario;
                ViewBag.ProductosVendidosUsuario = productosVendidosUsuario;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;

                return View(facturas);
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Error"] = "No tienes acceso a esta sección. Requerido: Nivel 1 o superior.";
                return RedirectToAction("Login", "Auth");
            }
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
