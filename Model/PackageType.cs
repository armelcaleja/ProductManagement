using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Model
{
    public class PackageType
    {
        public int PackageTypeId { get; set; }

        [Required]
        public string PackageTypeName { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
