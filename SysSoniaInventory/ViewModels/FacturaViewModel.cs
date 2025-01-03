namespace SysSoniaInventory.ViewModels
{
    public class FacturaViewModel
    {
        public int Id { get; set; }
        public string NameUser { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public decimal TotalFactura { get; set; }
        public List<DetalleFacturaViewModel> Detalles { get; set; }
    }
    public class DetalleFacturaViewModel
    {
        public string? CodigoProducto { get; set; }
        public string NameProduct { get; set; }
        public int CantidadProduct { get; set; }
    }
    public class UsuariosVentasViewModel
    {
        public string NameUser { get; set; }
        public decimal TotalFacturas { get; set; }
    }



}
