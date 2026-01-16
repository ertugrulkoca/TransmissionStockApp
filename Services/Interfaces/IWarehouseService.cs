using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<OperationResult<List<WarehouseViewModel>>> GetAllAsync();
        Task<OperationResult<WarehouseViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<WarehouseViewModel>> CreateAsync(WarehouseCreateDto dto);
        Task<OperationResult<WarehouseViewModel>> UpdateAsync(WarehouseUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}
