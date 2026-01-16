using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface IShelfService
    {
        Task<OperationResult<List<ShelfViewModel>>> GetAllAsync();
        Task<OperationResult<ShelfViewModel?>> GetByIdAsync(int id);

        Task<OperationResult<List<ShelfViewModel>>> GetByWarehouseAsync(int warehouseId); // ✅ NEW

        Task<OperationResult<ShelfViewModel>> CreateAsync(ShelfCreateDto dto);
        Task<OperationResult<ShelfViewModel>> UpdateAsync(ShelfUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}
