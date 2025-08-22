using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ITransmissionStockService
    {
        Task<OperationResult<List<TransmissionStockViewModel>>> GetAllAsync();
        Task<OperationResult<TransmissionStockViewModel>> GetByIdAsync(int id);
        Task<OperationResult<TransmissionStockViewModel>> CreateAsync(TransmissionStockCreateDto dto);
        Task<OperationResult<TransmissionStockViewModel>> UpdateAsync(int id, TransmissionStockUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
        Task<PagedResult<TransmissionStockViewModel>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<OperationResult<DuplicateCheckViewModel>> CheckDuplicateAsync(int transmissionBrandId, string sparePartNo, int transmissionStatusId);
    }

}
