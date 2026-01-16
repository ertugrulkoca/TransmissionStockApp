namespace TransmissionStockApp.Models.Entities
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int VehicleBrandId { get; set; }
        public VehicleBrand VehicleBrand { get; set; } = null!;

        public ICollection<TransmissionStock> TransmissionStocks { get; set; } = new List<TransmissionStock>();
    }
}
