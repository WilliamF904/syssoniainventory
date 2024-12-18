using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelProveedor
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(60), Unicode(false)]
        public string Name { get; set; }

        [StringLength(250), Unicode(false)]
        public string? Description { get; set; }

        public int? Tel { get; set; }

        [StringLength(75), Unicode(false)]
        public string? Email { get; set; }

        public virtual ICollection<ModelProduct> Product { get; set; } = new List<ModelProduct>();
    }
}
