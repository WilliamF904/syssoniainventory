using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelDetalleCompra
    {
        [Key]
        public int Id { get; set; }

        public int IdCompra { get; set; }

        public int IdProduct { get; set; }

        [MaxLength(75), Unicode(false)]
        public string? MarcaProducto { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameProducto { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? CodigoProducto { get; set; }

        [Required]
        public int CantidadProduct { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceCompraUnitario { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceTotal { get; set; }

        [ForeignKey("IdCompra")]
        public virtual ModelCompra? IdCompraNavigation { get; set; }

     
        public bool UpdatePrice { get; set; }
    }
}
