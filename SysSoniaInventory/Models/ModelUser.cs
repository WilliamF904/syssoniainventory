using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SysSoniaInventory.Models
{
    public class ModelUser
    {
        [Key]
        public int Id { get; set; }

        public int IdRol { get; set; }

        public int IdSucursal { get; set; }

        public int? Tel { get; set; }

        [Required, StringLength(50), Unicode(false)]
        public string? Name { get; set; }

        [Required, StringLength(50), Unicode(false)]
        public string? LastName { get; set; }

        [Required, StringLength(75), Unicode(false)]
        public string? Email { get; set; }

        [Required, StringLength(64), Unicode(false)]
        public string? Password { get; set; }

        public byte Estatus { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly? RegistrationDate { get; set; }

        [ForeignKey("IdRol")]
        public virtual ModelRol? IdRolNavigation { get; set; }

        [ForeignKey("IdSucursal")]
        public virtual ModelSucursal? IdSucursalNavigation { get; set; }
    }
}
