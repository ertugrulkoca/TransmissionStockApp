namespace TransmissionStockApp.Models.DTOs
{
    public class TransmissionStockCreateDto
    {
        public string SparePartNo { get; set; } = null!;
        public int TransmissionBrandId { get; set; }
        public string TransmissionCode { get; set; } = null!;
        public string TransmissionNumber { get; set; } = null!;
        public int? Year { get; set; }

        public int? VehicleBrandId { get; set; }
        public int? VehicleModelId { get; set; }
        public int TransmissionStatusId { get; set; }
        public int? DriveTypeId { get; set; }
        public string? Description { get; set; }

        public List<StockLocationQuantityDto>? StockLocations { get; set; } = new();
    }
}
