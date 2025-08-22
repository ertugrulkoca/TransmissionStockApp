namespace TransmissionStockApp.Models.ViewModels
{
    public class DuplicateCheckViewModel
    {
        public bool Exists { get; set; }
        public int? ExistingId { get; set; }
        public string? TransmissionBrandName { get; set; }
        public string? TransmissionStatusName { get; set; }
        public string? SparePartNo { get; set; }
        public int TotalQuantity { get; set; }
        public string ShelfSummary { get; set; } = "";
    }
}
