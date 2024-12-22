using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.DataAccess;
using SysSoniaInventory.Models;
using SysSoniaInventory.ViewModels;
using System.Diagnostics;

namespace SysSoniaInventory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;

        public HomeController(ILogger<HomeController> logger, DBContext context)
        {
            _logger = logger;
            _context = context;
        }

public IActionResult Index()
    {
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

        return View(facturas);
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
