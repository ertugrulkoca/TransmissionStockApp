using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class StockLocationService : IStockLocationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StockLocationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<StockLocationViewModel>>> GetAllAsync()
        {
            var locations = await _context.StockLocations.AsNoTracking().ToListAsync();
            var viewModels = _mapper.Map<List<StockLocationViewModel>>(locations);
            return OperationResult<List<StockLocationViewModel>>.Ok(viewModels);
        }

        public async Task<OperationResult<StockLocationViewModel?>> GetByIdAsync(int id)
        {
            var location = await _context.StockLocations.FindAsync(id);
            if (location == null)
                return OperationResult<StockLocationViewModel?>.Fail("Kayıt bulunamadı.");

            var viewModel = _mapper.Map<StockLocationViewModel>(location);
            return OperationResult<StockLocationViewModel?>.Ok(viewModel);
        }

        public async Task<OperationResult<StockLocationViewModel>> CreateAsync(StockLocationCreateDto dto)
        {
            var stockLocation = await _context.StockLocations.FirstOrDefaultAsync(s => s.ShelfCode == dto.ShelfCode);

            if (stockLocation == null)
            {
                stockLocation = new StockLocation { ShelfCode = dto.ShelfCode };
                _context.StockLocations.Add(stockLocation);
                await _context.SaveChangesAsync(); // yeni raf eklendiğinde Id oluşturulsun
            }


            var viewModel = _mapper.Map<StockLocationViewModel>(stockLocation);
            return OperationResult<StockLocationViewModel>.Ok(viewModel);
        }


        public async Task<OperationResult<StockLocationViewModel>> UpdateAsync(StockLocationUpdateDto dto)
        {
            var existing = await _context.StockLocations.FindAsync(dto.Id);
            if (existing == null)
                return OperationResult<StockLocationViewModel>.Fail("Kayıt bulunamadı.");

            existing.ShelfCode = dto.ShelfCode;
            await _context.SaveChangesAsync();

            var viewModel = _mapper.Map<StockLocationViewModel>(existing);
            return OperationResult<StockLocationViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            var entity = await _context.StockLocations.FindAsync(id);
            if (entity == null)
                return OperationResult<bool>.Fail("Kayıt bulunamadı.");

            try
            {
                _context.StockLocations.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException)
            {
                // FK Restrict sebebiyle
                return OperationResult<bool>.Fail("Bu lokasyonda bağlı stok varken silinemez. Önce stok bağlantılarını kaldırın.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

    }
}
