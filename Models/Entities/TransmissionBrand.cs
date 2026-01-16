namespace TransmissionStockApp.Models.Entities
{
    public class TransmissionBrand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<TransmissionStock> TransmissionStocks { get; set; } = new List<TransmissionStock>();
    }
}
