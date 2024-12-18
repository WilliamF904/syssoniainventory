using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelRol
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30), Unicode(false)]
        public string Name { get; set; }

        [Required, StringLength(30), Unicode(false)]
        public string AccessTipe { get; set; }

        public virtual ICollection<ModelUser> User { get; set; } = new List<ModelUser>();
    }
}
