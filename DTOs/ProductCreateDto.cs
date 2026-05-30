namespace ProductManagement.DTOs
{
    public class ProductCreateDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int ProductPrice { get; set; }
        public int CreatedBy { get; set; }
    }
}
