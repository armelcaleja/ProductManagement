using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Domain.Entities
{
    public class Item
    {
        public int ItemId { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
