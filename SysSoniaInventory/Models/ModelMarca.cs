using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;




namespace SysSoniaInventory.Models
{
    public class ModelMarca
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(75), Unicode(false)]
        public string Name { get; set; }

        [StringLength(250), Unicode(false)]
        public string? Description { get; set; }

        public virtual ICollection<ModelProduct> Product { get; set; } = new List<ModelProduct>();
    }
}
