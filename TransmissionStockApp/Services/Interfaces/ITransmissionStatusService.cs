using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ITransmissionStatusService
    {
        Task<OperationResult<List<TransmissionStatusViewModel>>> GetAllAsync();
        Task<OperationResult<TransmissionStatusViewModel?>> GetByIdAsync(int id);
        Task<OperationResult<TransmissionStatusViewModel>> CreateAsync(TransmissionStatusCreateDto dto);
        Task<OperationResult<TransmissionStatusViewModel>> UpdateAsync(TransmissionStatusUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}
