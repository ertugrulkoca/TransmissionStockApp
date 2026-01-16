namespace TransmissionStockApp.Models.DTOs
{
    public class TransmissionStatusUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ShelfIdQuantityDto> Shelves { get; set; } = new();

    }
}
