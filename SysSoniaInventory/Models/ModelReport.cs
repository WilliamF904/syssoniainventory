using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysSoniaInventory.Models
{
    public class ModelReport
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100), Unicode(false)]
        public string TypeReport { get; set; }

        [Required, StringLength(1000), Unicode(false)]
        public string Description { get; set; }

        [Required, StringLength(30), Unicode(false)]
        public string Estatus { get; set; }

        [StringLength(100), Unicode(false)]
        public string NameUser { get; set; }

        [StringLength(1000), Unicode(false)]
        public string ComentaryUser { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly StarDate { get; set; }

        [Required, Column(TypeName = "timeonly")]
        public TimeOnly StarTime { get; set; }

        [Column(TypeName = "dateonly")]
        public DateOnly EndDate { get; set; }

        [Column(TypeName = "timeonly")]
        public TimeOnly EndTime { get; set; }


        public int? IdRelation { get; set; }
    }
}
