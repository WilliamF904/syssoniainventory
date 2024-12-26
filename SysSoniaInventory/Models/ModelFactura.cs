using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysSoniaInventory.Models
{
    public class ModelFactura
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(75), Unicode(false)]
        public string NameSucursal { get; set; }

        [Required, StringLength(100), Unicode(false)]
        public string NameUser { get; set; }

        [StringLength(75), Unicode(false)]
        public string? NameClient { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly Date { get; set; }

        [Required, Column(TypeName = "timeonly")]
        public TimeOnly Time { get; set; }



        public virtual ICollection<ModelDetalleFactura> DetalleFactura { get; set; } = new List<ModelDetalleFactura>();
    }
}
