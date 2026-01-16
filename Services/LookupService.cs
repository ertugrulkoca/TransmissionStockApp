using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class LookupService : ILookupService
    {
        private readonly AppDbContext _context;

        public LookupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LookupDataViewModel> GetLookupDataAsync()
        {
            var viewModel = new LookupDataViewModel
            {
                TransmissionBrands = await _context.TransmissionBrands
                    .AsNoTracking()
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                VehicleBrands = await _context.VehicleBrands
                    .AsNoTracking()
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                VehicleModels = await _context.VehicleModels
                    .AsNoTracking()
                    .Select(x => new VehicleModelDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        VehicleBrandId = x.VehicleBrandId
                    })
                    .ToListAsync(),

                DriveTypes = await _context.TransmissionDriveTypes
                    .AsNoTracking()
                    .Select(x => new DriveTypeDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                TransmissionStatuses = await _context.TransmissionStatuses
                    .AsNoTracking()
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                // ✅ YENİ: Warehouses
                Warehouses = await _context.Warehouses
                    .AsNoTracking()
                    .Select(w => new WarehouseLookupDto
                    {
                        Id = w.Id,
                        Name = w.Name
                    })
                    .ToListAsync(),

                // ✅ YENİ: Shelves (Warehouse ile birlikte)
                Shelves = await _context.Shelves
                    .AsNoTracking()
                    .Include(s => s.Warehouse)
                    .Select(s => new ShelfLookupDto
                    {
                        Id = s.Id,
                        ShelfCode = s.ShelfCode,
                        WarehouseId = s.WarehouseId,
                        WarehouseName = s.Warehouse.Name
                    })
                    .ToListAsync()
            };

            return viewModel;
        }
    }
}
