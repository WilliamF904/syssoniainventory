using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(60), Unicode(false)]
        public string Name { get; set; }

        public virtual ICollection<ModelProduct> Product { get; set; } = new List<ModelProduct>();
    }
}
