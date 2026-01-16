using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ITransmissionBrandService
    {
        Task<OperationResult<List<TransmissionBrand>>> GetAllAsync(CancellationToken ct = default);
        Task<OperationResult<TransmissionBrand?>> GetByIdAsync(int id);
        Task<OperationResult<TransmissionBrand>> CreateAsync(TransmissionBrandCreateDto dto);
        Task<OperationResult<TransmissionBrand>> UpdateAsync(TransmissionBrandUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}
