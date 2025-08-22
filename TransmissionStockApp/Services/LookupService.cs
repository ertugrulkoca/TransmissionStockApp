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
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                VehicleBrands = await _context.VehicleBrands
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),


                VehicleModels = await _context.VehicleModels
                    .Select(x => new VehicleModelDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        VehicleBrandId = x.VehicleBrandId
                    })
                    .ToListAsync(),

                
                DriveTypes = await _context.TransmissionDriveTypes
                    .Select(x => new DriveTypeDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                TransmissionStatuses = await _context.TransmissionStatuses
                    .Select(x => new IdNameDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                ShelfCodes = await _context.StockLocations
                    .Select(x => x.ShelfCode)
                    .Distinct()
                    .ToListAsync()
            };

            return viewModel;
        }
    }

}
