using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Domain.Entities
{
    public class PackageType
    {
        public int PackageTypeId { get; set; }

        [Required]
        public string PackageTypeName { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
