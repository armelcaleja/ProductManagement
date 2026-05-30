using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagement.Model
{
    public class Package
    {
        public int PackageId { get; set; }

        public int ProductId { get; set; }

        public int? ParentPackageId { get; set; }

        public int PackageTypeId { get; set; }

        public int CreatedBy { get; set; }

        public List<Item> Items { get; set; } = new();

        [ForeignKey("ParentPackageId")]
        public List<Package> PackageList { get; set; } = new();
    }
}
