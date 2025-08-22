using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface IStockLocationService
    {
        Task<OperationResult<List<StockLocationViewModel>>> GetAllAsync();
        Task<OperationResult<StockLocationViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<StockLocationViewModel>> CreateAsync(StockLocationCreateDto dto);
        Task<OperationResult<StockLocationViewModel>> UpdateAsync(StockLocationUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}
