namespace TransmissionStockApp.Models.Entities
{
    public class TransmissionStockLocation
    {
        public int TransmissionStockId { get; set; }
        public TransmissionStock TransmissionStock { get; set; } = null!;

        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; } = null!;

        public int Quantity { get; set; }
    }

}
