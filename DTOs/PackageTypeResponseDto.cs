namespace ProductManagement.DTOs
{
    public class PackageTypeResponseDto
    {
        public int PackageTypeId { get; set; }
        public string PackageTypeName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }
}
