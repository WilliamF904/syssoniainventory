using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelSucursal
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(75), Unicode(false)]
        public string Name { get; set; }

        [StringLength(100), Unicode(false)]
        public string? Address { get; set; }


        public virtual ICollection<ModelUser> User { get; set; } = new List<ModelUser>();
    }
}
