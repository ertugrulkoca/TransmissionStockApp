namespace TransmissionStockApp.Models.DTOs
{
    public class ShelfCreateDto
    {
        public int WarehouseId { get; set; }   // ZORUNLU

        public string ShelfCode { get; set; } = null!;
    }

}
