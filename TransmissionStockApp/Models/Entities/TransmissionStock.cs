namespace TransmissionStockApp.Models.Entities
{
    public class TransmissionStock
    {
        public int Id { get; set; }
        public int TransmissionBrandId { get; set; }
        public TransmissionBrand TransmissionBrand { get; set; } = null!;

        public string SparePartNo { get; set; } = null!;
        public string TransmissionCode { get; set; } = null!;
        public string TransmissionNumber { get; set; } = null!;
        public int? Year { get; set; }

        public int? VehicleBrandId { get; set; }
        public VehicleBrand? VehicleBrand { get; set; }
        public int? VehicleModelId { get; set; }
        public VehicleModel? VehicleModel { get; set; }

        public int TransmissionStatusId { get; set; }
        public TransmissionStatus TransmissionStatus { get; set; } = null!;

        public int? TransmissionDriveTypeId { get; set; }
        public TransmissionDriveType? TransmissionDriveType { get; set; }

        public ICollection<TransmissionStockLocation> TransmissionStockLocations { get; set; } = new List<TransmissionStockLocation>();
        public string? Description { get; set; }
    }
}
