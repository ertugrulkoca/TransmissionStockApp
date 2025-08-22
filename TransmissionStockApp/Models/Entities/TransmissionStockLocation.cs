namespace TransmissionStockApp.Models.Entities
{
    public class TransmissionStockLocation
    {
        public int TransmissionStockId { get; set; }
        public TransmissionStock TransmissionStock { get; set; } = null!;

        public int StockLocationId { get; set; }
        public StockLocation StockLocation { get; set; } = null!;

        public int Quantity { get; set; }
    }

}
