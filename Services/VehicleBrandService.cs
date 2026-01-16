using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Repositories;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class VehicleBrandService : IVehicleBrandService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITransmissionStockRepository _repository;

        public VehicleBrandService(AppDbContext context, IMapper mapper, ITransmissionStockRepository repository)
        {
            _context = context;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<OperationResult<List<VehicleBrandViewModel>>> GetAllAsync()
        {
            try
            {
                var brands = await _context.VehicleBrands.OrderBy(b => b.Name).ToListAsync();
                var vmList = _mapper.Map<List<VehicleBrandViewModel>>(brands);
                return OperationResult<List<VehicleBrandViewModel>>.Ok(vmList);
            }
            catch (Exception ex)
            {
                return OperationResult<List<VehicleBrandViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<VehicleBrandViewModel>> CreateAsync(VehicleBrandCreateDto dto)
        {
            try
            {
                var exists = await _context.VehicleBrands
                .AsNoTracking()
                .AnyAsync(w => w.Name == dto.Name);

                if (exists)
                    return OperationResult<VehicleBrandViewModel>.Fail("Bu depo adı zaten mevcut.");


                var brand = _mapper.Map<VehicleBrand>(dto);
                _context.VehicleBrands.Add(brand);
                await _context.SaveChangesAsync();

                var vm = _mapper.Map<VehicleBrandViewModel>(brand);
                return OperationResult<VehicleBrandViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleBrandViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<VehicleBrandViewModel>> UpdateAsync(VehicleBrandUpdateDto dto)
        {
            try
            {
                var brand = await _context.VehicleBrands.FindAsync(dto.Id);
                if (brand == null)
                    return OperationResult<VehicleBrandViewModel>.Fail("Marka bulunamadı");

                brand.Name = dto.Name;
                await _context.SaveChangesAsync();

                var vm = _mapper.Map<VehicleBrandViewModel>(brand);
                return OperationResult<VehicleBrandViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleBrandViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var brand = await _context.VehicleBrands.FindAsync(id);
                if (brand == null)
                    return OperationResult<bool>.Fail("Marka bulunamadı");

                _context.VehicleBrands.Remove(brand);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }
    public async Task<OperationResult<VehicleBrandViewModel?>> GetByIdAsync(int id)
    {
        try
        {
            var brand = await _context.VehicleBrands.FindAsync(id);
            if (brand == null)
                return OperationResult<VehicleBrandViewModel?>.Fail("Marka bulunamadı");

            var vm = _mapper.Map<VehicleBrandViewModel>(brand);
            return OperationResult<VehicleBrandViewModel?>.Ok(vm);
        }
        catch (Exception ex)
        {
            return OperationResult<VehicleBrandViewModel?>.Fail(ex.Message);
        }
    }

}

}