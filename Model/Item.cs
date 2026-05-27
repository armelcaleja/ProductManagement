using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Model
{
    public class Item
    {
        public int ItemId { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public int CreatedBy { get; set; }
    }
}
