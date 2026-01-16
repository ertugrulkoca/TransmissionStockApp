namespace TransmissionStockApp.Models.ViewModels
{
    public class TransmissionStockLocationViewModel
    {
        public int ShelfId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
