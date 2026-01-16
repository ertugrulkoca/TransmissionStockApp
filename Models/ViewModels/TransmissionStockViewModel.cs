namespace TransmissionStockApp.Models.ViewModels
{
    public class TransmissionStockViewModel
    {
        public int Id { get; set; }
        public string SparePartNo { get; set; } = null!;
        public int TransmissionBrandId { get; set; }
        public string? TransmissionBrandName { get; set; }

        public string TransmissionCode { get; set; } = null!;
        public string TransmissionNumber { get; set; } = null!;
        public int? Year { get; set; }

        public int? VehicleBrandId { get; set; }
        public string? VehicleBrandName { get; set; }

        public int? VehicleModelId { get; set; }
        public string? VehicleModelName { get; set; }

        public int? DriveTypeId { get; set; }
        public string? DriveTypeName{ get; set; }

        public int TransmissionStatusId { get; set; }
        public string? TransmissionStatusName { get; set; }

        public string? Description { get; set; }

        public List<ShelfViewModel> Shelves { get; set; } = new();

        public string ShelfSummary { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
    }
}
