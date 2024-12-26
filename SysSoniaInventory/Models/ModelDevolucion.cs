using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelDevolucion
    {
        [Key]
        public int Id { get; set; }

        public int IdFactura { get; set; }

        [Required, MaxLength(75), Unicode(false)]
        public string NameSucursal { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameUser { get; set; }

        [MaxLength(75), Unicode(false)]
        public string NameClient { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly Date { get; set; }

        [Required, Column(TypeName = "timeonly")]
        public TimeOnly Time { get; set; }


        public virtual ICollection<ModelDetalleDevolucion> DetalleDevolucion { get; set; } = new List<ModelDetalleDevolucion>();

        [ForeignKey("IdFactura")]
        public virtual ModelFactura? IdFacturaNavigation { get; set; }
    }
}
