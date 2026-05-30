namespace ProductManagement.DTOs
{
    public class PackageTypeCreateDto
    {
        public string PackageTypeName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }
}
