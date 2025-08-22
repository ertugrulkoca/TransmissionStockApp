using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface IVehicleModelService
    {
        Task<OperationResult<List<VehicleModelViewModel>>> GetByBrandIdAsync(int brandId);
        Task<OperationResult<VehicleModelViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<VehicleModelViewModel>> CreateAsync(VehicleModelCreateDto dto);
        Task<OperationResult<VehicleModelViewModel>> UpdateAsync(VehicleModelUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}