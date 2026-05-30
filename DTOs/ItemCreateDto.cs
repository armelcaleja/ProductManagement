namespace ProductManagement.DTOs
{
    public class ItemCreateDto
    {
        public string ItemName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }
}
