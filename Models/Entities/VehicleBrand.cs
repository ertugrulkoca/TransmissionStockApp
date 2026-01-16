namespace TransmissionStockApp.Models.Entities
{
    public class VehicleBrand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
        public ICollection<TransmissionStock> TransmissionStocks { get; set; } = new List<TransmissionStock>();
    }
}
