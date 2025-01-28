using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelCompra
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(75), Unicode(false)]
        public string NameSucursal { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameUser { get; set; }

        [Required, MaxLength(250), Unicode(false)]
        public string Description { get; set; }

        [MaxLength(75), Unicode(false)]
        public string? NameProveedor { get; set; }

        [MaxLength(75), Unicode(false)]
        public string? CodigoFactura { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly Date { get; set; }

        [Required, Column(TypeName = "timeonly")]
        public TimeOnly Time { get; set; }

        public virtual ICollection<ModelDetalleCompra> DetalleCompra { get; set; } = new List<ModelDetalleCompra>();
    }
}
