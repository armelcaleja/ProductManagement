namespace ProductManagement.DTOs
{
    public class PackageResponseDto
    {
        public int PackageId { get; set; }
        public int ProductId { get; set; }
        public int? ParentPackageId { get; set; }
        public int PackageTypeId { get; set; }
        public string PackageTypeName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public List<ItemResponseDto> Items { get; set; } = new();
        public List<PackageResponseDto> PackageList { get; set; } = new();
    }
}
