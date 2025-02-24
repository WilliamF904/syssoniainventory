﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelDetalleDevolucion
    {
        [Key]
        public int Id { get; set; }
      
        public int IdDevolucion { get; set; }
     
        public int IdProduct { get; set; }

        [Required, MaxLength(100), Unicode(false)]
        public string NameProduct { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? CodigoProducto { get; set; }

        [Required]
        public int CantidadProduct { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceReembolso { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal PriceTotalReembolso { get; set; }

      
        public bool StockD { get; set; }

        [ForeignKey("IdDevolucion")]
        public virtual ModelDevolucion? IdDevolucionNavigation { get; set; }
    }
}
