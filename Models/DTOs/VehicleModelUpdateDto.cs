namespace TransmissionStockApp.Models.DTOs
{
    public class VehicleModelUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int VehicleBrandId { get; set; }
    }
}
