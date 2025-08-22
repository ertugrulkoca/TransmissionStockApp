namespace TransmissionStockApp.Models.DTOs
{
    public class TransmissionStockLocationCreateDto
    {
        public int TransmissionStockId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
