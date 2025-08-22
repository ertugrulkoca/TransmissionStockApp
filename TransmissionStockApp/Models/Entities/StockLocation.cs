namespace TransmissionStockApp.Models.Entities
{
    public class StockLocation
    {
        public int Id { get; set; }
        public string ShelfCode { get; set; } = null!;

        public ICollection<TransmissionStockLocation> TransmissionStockLocations { get; set; } = new List<TransmissionStockLocation>();
    }
}
