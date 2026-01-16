namespace TransmissionStockApp.Models.ViewModels
{
    public class ShelfViewModel
    {
        public int Id { get; set; }
        public string ShelfCode { get; set; } = null!;
        public int Quantity { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
    }
}
