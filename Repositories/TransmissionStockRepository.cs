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
                .AsNoTracking()
                .AsSplitQuery()
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(vm => vm.VehicleBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.Shelf)
                        .ThenInclude(s => s.Warehouse)
                .OrderByDescending(ts => ts.Id)
                .ToListAsync();
        }




        public async Task<TransmissionStock?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(vm => vm.VehicleBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.Shelf)
                        .ThenInclude(sl => sl.Warehouse) // ✅ bunu ekle
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }


    }

}
