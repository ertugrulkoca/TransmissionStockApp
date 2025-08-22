using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Helpers;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class TransmissionBrandService : ITransmissionBrandService
    {
        private readonly AppDbContext _context;

        public TransmissionBrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<List<TransmissionBrand>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var list = await _context.TransmissionBrands
                    .OrderBy(b => b.Name)
                    .ToListAsync();

                return OperationResult<List<TransmissionBrand>>.Ok(list);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TransmissionBrand>>.Fail("Markalar listelenemedi.");
            }
        }

        public async Task<OperationResult<TransmissionBrand?>> GetByIdAsync(int id)
        {
            var brand = await _context.TransmissionBrands.FindAsync(id);
            return brand != null
                ? OperationResult<TransmissionBrand?>.Ok(brand)
                : OperationResult<TransmissionBrand?>.Fail("Marka bulunamadı");
        }

        public async Task<OperationResult<TransmissionBrand>> CreateAsync(TransmissionBrandCreateDto dto)
        {
            try
            {
                var brand = new TransmissionBrand { Name = dto.Name };
                _context.TransmissionBrands.Add(brand);
                await _context.SaveChangesAsync();
                return OperationResult<TransmissionBrand>.Ok(brand);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionBrand>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionBrand>> UpdateAsync(TransmissionBrandUpdateDto dto)
        {
            try
            {
                var brand = new TransmissionBrand { Id = dto.Id, Name = dto.Name };
                var existing = await _context.TransmissionBrands.FindAsync(brand.Id);
                if (existing == null)
                    return OperationResult<TransmissionBrand>.Fail("Marka bulunamadı");

                existing.Name = brand.Name;
                await _context.SaveChangesAsync();
                return OperationResult<TransmissionBrand>.Ok(existing);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionBrand>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            var entity = await _context.TransmissionBrands.FindAsync(id);
            if (entity == null)
                return OperationResult<bool>.Fail("Kayıt bulunamadı.");

            try
            {
                _context.TransmissionBrands.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyViolation(ex))
            {
                return OperationResult<bool>.Fail(
                    "Bu şanzıman markası bazı stok kayıtlarında kullanılıyor. " +
                    "Önce bu markayı kullanan stokları güncelleyin veya kaldırın.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

    }

}
