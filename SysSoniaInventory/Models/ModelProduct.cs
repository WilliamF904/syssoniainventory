using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelProduct
    {
        [Key]
        public int Id { get; set; }

        public int IdCategory { get; set; }

        public int IdProveedor { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string Name { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal SalePrice { get; set; }

        [Required]
        public int Stock { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? Codigo { get; set; }

        [StringLength(100), Unicode(false)]
        public string? Url { get; set; }
       
        public byte Estatus { get; set; }



        [ForeignKey("IdCategory")]
        public virtual ModelCategory? IdCategoryNavigation { get; set; }

        [ForeignKey("IdProveedor")]
        public virtual ModelProveedor? IdProveedorNavigation { get; set; }

    }
}
