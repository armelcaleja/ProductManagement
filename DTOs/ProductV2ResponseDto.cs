namespace ProductManagement.DTOs
{
    public class ProductV2ResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductPrice { get; set; }
        public List<PackageResponseDto> Packages { get; set; } = new();
    }
}
