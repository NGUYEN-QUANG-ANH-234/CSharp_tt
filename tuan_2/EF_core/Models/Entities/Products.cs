using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_core.Models.Entities 
{
    [Table("products")]
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string ProductDescription { get; set; }

        public void PrintInfo() => Console.WriteLine($"{ProductId} - {ProductName} - {ProductDescription} ");
    }
}