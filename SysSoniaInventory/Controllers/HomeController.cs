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
using System.Security.Claims;

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



            // Verifica si el usuario está autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                // Obtiene el valor del claim y lo convierte a int
                int userId;
                bool isParsed = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);

                // Si el claim se pudo convertir, asigna el valor, de lo contrario deja en 0
                var reportesMensaje = await _context.modelReport
                    .Where(r => r.TypeReport == "Mensaje o recordatorio" && r.IdRelation == userId)
                    .OrderBy(r => r.StarDate).ThenBy(r => r.StarTime)
                    .Take(5) // Limita la cantidad de reportes si es necesario
                    .ToListAsync();

                ViewBag.ReportesMensaje = reportesMensaje;
                ViewBag.MostrarBotonVerTodos = reportesMensaje.Count > 5;
            }


            if (User.HasClaim("AccessTipe", "Nivel 5") || User.HasClaim("AccessTipe", "Nivel 4"))
            {

                // Filtrar reportes no finalizados y excluir los de tipo "Mensaje o recordatorio"
                var reportesNoFinalizados = await _context.modelReport
                    .Where(r => r.Estatus != "Finalizado" && r.TypeReport != "Mensaje o recordatorio")
                    .OrderBy(r => r.StarDate).ThenBy(r => r.StarTime)
                    .Take(5)
                    .ToListAsync();

                // Filtrar reportes finalizados y excluir los de tipo "Mensaje o recordatorio"
                var reportesFinalizados = await _context.modelReport
                    .Where(r => r.Estatus == "Finalizado" && r.TypeReport != "Mensaje o recordatorio")
                    .OrderBy(r => r.StarDate).ThenBy(r => r.StarTime)
                    .Take(5 - reportesNoFinalizados.Count)
                    .ToListAsync();

                // Combinar ambas listas
                var reportesCombinados = reportesNoFinalizados.Concat(reportesFinalizados).ToList();

                // Pasar los reportes a la vista
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
                    TotalProduct = f.DetalleFactura.Sum(d => (int)d.CantidadProduct),
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
                        TotalFacturas = g.Sum(f => f.TotalFactura),
                        TotalProducts = g.Sum(f => f.TotalProduct)
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
                    usuario.TotalProducts,
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







        [AllowAnonymous]
        public IActionResult Nosotros()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error404()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error500()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





    }
}
