using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface IVehicleBrandService
    {
        Task<OperationResult<List<VehicleBrandViewModel>>> GetAllAsync();
        Task<OperationResult<VehicleBrandViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<VehicleBrandViewModel>> CreateAsync(VehicleBrandCreateDto dto);
        Task<OperationResult<VehicleBrandViewModel>> UpdateAsync(VehicleBrandUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}