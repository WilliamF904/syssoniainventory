using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelDetalleFactura
    {
        [Key]
        public int Id { get; set; }

        public int IdFactura { get; set; }

        public int IdProduct { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameProducto { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? CodigoProducto { get; set; }

        [Required]
        public int CantidadProduct { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal SalePriceUnitario { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorDescuento { get; set; } = 0;

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal SalePriceDescuento { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceTotal { get; set; }


        [JsonIgnore]
        [ForeignKey("IdFactura")]
        public virtual ModelFactura? IdFacturaNavigation { get; set; }

       
    }
}
