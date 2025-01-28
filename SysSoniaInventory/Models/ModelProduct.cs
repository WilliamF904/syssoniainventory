﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Categoria.")]
        public int IdCategory { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Proveedor.")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una marca.")]
        public int IdMarca{ get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string Name { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal SalePrice { get; set; }

        [Required]
        public int Stock { get; set; }
        public int LowStock { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? Codigo { get; set; }

        [StringLength(100), Unicode(false)]
        public string? Url { get; set; }
       
        public byte Estatus { get; set; }



        [ForeignKey("IdCategory")]
        public virtual ModelCategory? IdCategoryNavigation { get; set; }

        [ForeignKey("IdProveedor")]
        public virtual ModelProveedor? IdProveedorNavigation { get; set; }

        [ForeignKey("IdMarca")]
        public virtual ModelMarca? IdMarcanavigation { get; set; }
    }
}
