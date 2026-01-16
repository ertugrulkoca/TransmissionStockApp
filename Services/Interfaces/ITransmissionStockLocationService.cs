using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ITransmissionStockLocationService
    {
        Task<OperationResult<List<TransmissionStockLocationViewModel>>> GetByTransmissionStockIdAsync(int transmissionStockId);
        Task<OperationResult<bool>> AddAsync(int transmissionStockId, string shelfCode, int quantity);
        Task<OperationResult<bool>> UpdateQuantityAsync(int transmissionStockId, int shelfId, int newQuantity);
        Task<OperationResult<bool>> DeleteAsync(int transmissionStockId, int shelfId);
    }


}
