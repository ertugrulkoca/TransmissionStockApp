namespace TransmissionStockApp.Models.DTOs
{
    public class VehicleModelCreateDto
    {
        public string Name { get; set; } = null!;
        public int VehicleBrandId { get; set; }
    }
}
