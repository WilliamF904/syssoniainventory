using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SysSoniaInventory.Models
{
    public class ModelHistorialProduct
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100), Unicode(false)]
        public string? NameUser { get; set; }

        [Required]
        public int IdProduct { get; set; }

        [StringLength(100), Unicode(false)]
        public string? BeforeNameProduct { get; set; }

        [StringLength(100), Unicode(false)]
        public string? AfterNameProduct { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BeforePurchasePrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AfterPurchasePrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BeforeSalePrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AfterSalePrice { get; set; }

        public int? BeforeStock { get; set; }

        public int? AfterStock { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? BeforeCodigo { get; set; }

        [MaxLength(25), Unicode(false)]
        public string? AfterCodigo { get; set; }

        [Required, Column(TypeName = "dateonly")]
        public DateOnly Date { get; set; }

        [Required, Column(TypeName = "timeonly")]
        public TimeOnly Time { get; set; }

        [MaxLength(50)]
        public string RazonCambioAuto { get; set; }

        [MaxLength(250)]
        public string? DescriptionCambio { get; set; }

     
       
    }
}
