namespace TransmissionStockApp.Models.DTOs
{
    public class DuplicateCheckDto
    {
        public int TransmissionBrandId { get; set; }
        public string SparePartNo { get; set; } = null!;
        public int TransmissionStatusId { get; set; }
    }
}
