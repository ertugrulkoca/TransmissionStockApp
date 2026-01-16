namespace TransmissionStockApp.Models.ViewModels
{
    public class VehicleModelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int VehicleBrandId { get; set; }
        public string VehicleBrandName { get; set; } = null!;
    }
}
