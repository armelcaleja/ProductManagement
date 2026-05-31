namespace ProductManagement.Application.DTOs
{
    public class PackageCreateDto
    {
        public int ProductId { get; set; }
        public int? ParentPackageId { get; set; }
        public int PackageTypeId { get; set; }
    }
}
