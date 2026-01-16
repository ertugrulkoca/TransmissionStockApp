using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ITransmissionDriveTypeService
    {
        Task<OperationResult<List<TransmissionDriveTypeViewModel>>> GetAllAsync();
        Task<OperationResult<TransmissionDriveTypeViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<TransmissionDriveTypeViewModel>> CreateAsync(TransmissionDriveTypeCreateDto dto);
        Task<OperationResult<TransmissionDriveTypeViewModel>> UpdateAsync(TransmissionDriveTypeUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}
