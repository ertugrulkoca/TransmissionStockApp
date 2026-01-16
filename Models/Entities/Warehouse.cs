namespace TransmissionStockApp.Models.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
    }
}
