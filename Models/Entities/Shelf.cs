namespace TransmissionStockApp.Models.Entities
{
    public class Shelf
    {
        public int Id { get; set; }
        public string ShelfCode { get; set; } = null!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public ICollection<TransmissionStockLocation> TransmissionStockLocations { get; set; } = new List<TransmissionStockLocation>();
    }

}
