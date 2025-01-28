using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelDetalleDevolucion
    {
        [Key]
        public int Id { get; set; }
      
        public int IdDevolucion { get; set; }
     
        public int IdProduct { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameProduct { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? CodigoProducto { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal SalePriceUnitario { get; set; }

        [Required]
        public int CantidadProduct { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceReembolso { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceTotalReembolso { get; set; }

        [NotMapped] // Esto indica que no será parte de la base de datos.
        public string StockD { get; set; }

        [ForeignKey("IdDevolucion")]
        public virtual ModelDevolucion? IdDevolucionNavigation { get; set; }
    }
}
