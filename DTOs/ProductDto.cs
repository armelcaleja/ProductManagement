using ProductManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int ProductPrice { get; set; }

        public List<Package> Packages { get; set; } = new();
    }
}
