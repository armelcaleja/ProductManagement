namespace ProductManagement.Domain.Entities
{
    public class PackageItem
    {
        public int PackageItemId { get; set; }

        public int PackageId { get; set; }

        public int ItemId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
