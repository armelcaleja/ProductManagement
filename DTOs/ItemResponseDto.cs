namespace ProductManagement.DTOs
{
    public class ItemResponseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }
}
