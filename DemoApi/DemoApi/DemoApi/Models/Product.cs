using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoApi.Models
{
    public class Product
    {
        public Product() { }

        // Copy Constructor
        public Product(Product product)
        {
            ProductId = product.ProductId;
            UnitCost = product.UnitCost;
            Name = product.Name;
            Quantity = product.Quantity;
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1001,int.MaxValue,ErrorMessage = "Product Id has to be greater than 1000")]
        public int ProductId { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Name must contain atleast 4 characters")]
        [MaxLength(25, ErrorMessage = "Name must contain less than 25 characters")]
        public string Name { get; set; }

        [Required]
        [Range(5,1000000,ErrorMessage ="Cost has to be Greater than 5 and less than 10,00,000")]
        public decimal UnitCost { get; set; }

        [Required]
        [Range(5, 400, ErrorMessage = "Quantity has to be Greater than 5 and less than 400")]
        public int Quantity { get; set; }
    }
}
