namespace TransmissionStockApp.Models.ViewModels
{
    public class TransmissionStockLocationViewModel
    {
        public int StockLocationId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
