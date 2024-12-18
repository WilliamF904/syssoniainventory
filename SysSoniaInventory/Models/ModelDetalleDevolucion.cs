using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelDetalleDevolucion
    {
        [Key]
        public int Id { get; set; }
      
        public int IdDevolucion { get; set; }
     
        public int IdProduct { get; set; }

        [Required, MaxLength(100)]
        public string NameProduct { get; set; }

        [Required]
        public int CodigoProducto { get; set; }

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


        [ForeignKey("IdDevolucion")]
        public virtual ModelDevolucion? IdDevolucionNavigation { get; set; }
    }
}
