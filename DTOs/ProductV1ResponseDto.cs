using ProductManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs
{
    public class ProductV1ResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductPrice { get; set; }
        public int CreatedBy { get; set; }
    }
}
