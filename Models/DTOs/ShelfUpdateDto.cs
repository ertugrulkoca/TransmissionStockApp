namespace TransmissionStockApp.Models.DTOs
{
    public class ShelfUpdateDto
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }   // Taşıma ihtimali için

        public string ShelfCode { get; set; } = null!;
    }

}
