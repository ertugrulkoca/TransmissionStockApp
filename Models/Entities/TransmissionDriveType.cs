namespace TransmissionStockApp.Models.Entities
{
    public class TransmissionDriveType
    {
        public int Id { get; set; } // Primary key - otomatik artar
        public string Name { get; set; } = null!; // Açıklama (örn: "2 Çeker")

        public ICollection<TransmissionStock> TransmissionStocks { get; set; } = new List<TransmissionStock>();
    }
}
