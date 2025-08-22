using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class TransmissionStockLocationService : ITransmissionStockLocationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransmissionStockLocationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<TransmissionStockLocationViewModel>>> GetByTransmissionStockIdAsync(int transmissionStockId)
        {
            var list = await _context.TransmissionStockLocations
                .Include(tsl => tsl.StockLocation)
                .Where(tsl => tsl.TransmissionStockId == transmissionStockId)
                .ToListAsync();

            var viewModels = list.Select(tsl => new TransmissionStockLocationViewModel
            {
                StockLocationId = tsl.StockLocationId,
                ShelfCode = tsl.StockLocation.ShelfCode,
                Quantity = tsl.Quantity
            }).ToList();

            return OperationResult<List<TransmissionStockLocationViewModel>>.Ok(viewModels);
        }

        public async Task<OperationResult<bool>> AddAsync(int transmissionStockId, string shelfCode, int quantity)
        {
            var transmissionStock = await _context.TransmissionStocks.FindAsync(transmissionStockId);
            if (transmissionStock == null)
                return OperationResult<bool>.Fail("Şanzıman bulunamadı.");

            var stockLocation = await _context.StockLocations
                .FirstOrDefaultAsync(s => s.ShelfCode == shelfCode);

            if (stockLocation == null)
            {
                stockLocation = new StockLocation { ShelfCode = shelfCode };
                _context.StockLocations.Add(stockLocation);
                await _context.SaveChangesAsync();
            }

            var exists = await _context.TransmissionStockLocations
                .AnyAsync(tsl => tsl.TransmissionStockId == transmissionStockId && tsl.StockLocationId == stockLocation.Id);

            if (exists)
                return OperationResult<bool>.Fail("Bu raf bu şanzıman için zaten tanımlı.");

            var tsl = new TransmissionStockLocation
            {
                TransmissionStockId = transmissionStockId,
                StockLocationId = stockLocation.Id,
                Quantity = quantity
            };

            _context.TransmissionStockLocations.Add(tsl);
            await _context.SaveChangesAsync();

            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> UpdateQuantityAsync(int transmissionStockId, int stockLocationId, int newQuantity)
        {
            var tsl = await _context.TransmissionStockLocations
                .FirstOrDefaultAsync(x => x.TransmissionStockId == transmissionStockId && x.StockLocationId == stockLocationId);

            if (tsl == null)
                return OperationResult<bool>.Fail("Kayıt bulunamadı.");

            tsl.Quantity = newQuantity;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Ok(true);
        }

        public async Task<OperationResult<bool>> DeleteAsync(int transmissionStockId, int stockLocationId)
        {
            var tsl = await _context.TransmissionStockLocations
                .FirstOrDefaultAsync(x => x.TransmissionStockId == transmissionStockId && x.StockLocationId == stockLocationId);

            if (tsl == null)
                return OperationResult<bool>.Fail("Kayıt bulunamadı.");

            _context.TransmissionStockLocations.Remove(tsl);
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Ok(true);
        }
    }
}
