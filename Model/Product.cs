using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Model
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int ProductPrice { get; set; }

        public int CreatedBy { get; set; }

        public List<Package> Packages { get; set; } = new();
    }
}
