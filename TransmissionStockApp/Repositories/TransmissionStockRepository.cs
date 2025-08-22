using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.Entities;

namespace TransmissionStockApp.Repositories
{
    public class TransmissionStockRepository : Repository<TransmissionStock>, ITransmissionStockRepository
    {
        public TransmissionStockRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<TransmissionStock>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(ts => ts.VehicleBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.StockLocation)
                .ToListAsync();
        }

        public async Task<TransmissionStock?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(ts => ts.VehicleBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.StockLocation)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }
    }

}
