using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public VehicleModelService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VehicleModelViewModel>>> GetByBrandIdAsync(int vehicleBrandId)
        {
            try
            {
                var models = await _context.VehicleModels
                    .Where(m => m.VehicleBrandId == vehicleBrandId)
                    .OrderBy(m => m.Name)
                    .ToListAsync();

                var vmList = _mapper.Map<List<VehicleModelViewModel>>(models);
                return OperationResult<List<VehicleModelViewModel>>.Ok(vmList);
            }
            catch (Exception ex)
            {
                return OperationResult<List<VehicleModelViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<VehicleModelViewModel>> CreateAsync(VehicleModelCreateDto dto)
        {
            try
            {
                var model = _mapper.Map<VehicleModel>(dto);
                _context.VehicleModels.Add(model);
                await _context.SaveChangesAsync();

                var vm = _mapper.Map<VehicleModelViewModel>(model);
                return OperationResult<VehicleModelViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleModelViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<VehicleModelViewModel>> UpdateAsync(VehicleModelUpdateDto dto)
        {
            try
            {
                var model = await _context.VehicleModels.FindAsync(dto.Id);
                if (model == null)
                    return OperationResult<VehicleModelViewModel>.Fail("Model bulunamadı");

                model.Name = dto.Name;
                model.VehicleBrandId = dto.VehicleBrandId;

                await _context.SaveChangesAsync();
                var vm = _mapper.Map<VehicleModelViewModel>(model);
                return OperationResult<VehicleModelViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleModelViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var model = await _context.VehicleModels.FindAsync(id);
                if (model == null)
                    return OperationResult<bool>.Fail("Model bulunamadı");

                _context.VehicleModels.Remove(model);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<VehicleModelViewModel?>> GetByIdAsync(int id)
        {
            try
            {
                var model = await _context.VehicleModels
                    .Include(m => m.VehicleBrand)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                    return OperationResult<VehicleModelViewModel?>.Fail("Model bulunamadı");

                var vm = _mapper.Map<VehicleModelViewModel>(model);
                return OperationResult<VehicleModelViewModel?>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleModelViewModel?>.Fail(ex.Message);
            }
        }

    }
}