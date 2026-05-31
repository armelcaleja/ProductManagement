using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int ProductPrice { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public List<Package> Packages { get; set; } = new();
    }
}
