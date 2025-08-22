namespace TransmissionStockApp.Models.ViewModels
{
    public class LookupDataViewModel
    {
        public List<IdNameDto> TransmissionBrands { get; set; } = new();
        public List<IdNameDto> VehicleBrands { get; set; } = new();
        public List<VehicleModelDto> VehicleModels { get; set; } = new();
        public List<DriveTypeDto> DriveTypes { get; set; } = new();
        public List<IdNameDto> TransmissionStatuses { get; set; } = new();
        public List<string> ShelfCodes { get; set; } = new();
    }

    public class IdNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class DriveTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class VehicleModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VehicleBrandId { get; set; }
    }

}
