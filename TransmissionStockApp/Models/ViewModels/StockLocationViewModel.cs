namespace TransmissionStockApp.Models.ViewModels
{
    public class StockLocationViewModel
    {
        public int Id { get; set; }
        public string ShelfCode { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
