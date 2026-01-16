using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class TransmissionDriveTypeService : ITransmissionDriveTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransmissionDriveTypeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<TransmissionDriveTypeViewModel>>> GetAllAsync()
        {
            try
            {
                var list = await _context.TransmissionDriveTypes
                    .OrderBy(dt => dt.Id)
                    .ToListAsync();

                var vmList = _mapper.Map<List<TransmissionDriveTypeViewModel>>(list);
                return OperationResult<List<TransmissionDriveTypeViewModel>>.Ok(vmList);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TransmissionDriveTypeViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionDriveTypeViewModel?>> GetByIdAsync(int id)
        {
            var entity = await _context.TransmissionDriveTypes.FindAsync(id);
            if (entity == null)
                return OperationResult<TransmissionDriveTypeViewModel?>.Fail("Çekiş tipi bulunamadı");

            var vm = _mapper.Map<TransmissionDriveTypeViewModel>(entity);
            return OperationResult<TransmissionDriveTypeViewModel?>.Ok(vm);
        }

        public async Task<OperationResult<TransmissionDriveTypeViewModel>> CreateAsync(TransmissionDriveTypeCreateDto dto)
        {
            try
            {
                var exists = await _context.TransmissionDriveTypes
                .AsNoTracking()
                .AnyAsync(w => w.Name == dto.Name);

                if (exists)
                    return OperationResult<TransmissionDriveTypeViewModel>.Fail("Bu çekiş türü zaten mevcut.");

                var driveType = new TransmissionDriveType { Name = dto.Name };
                var entity = _mapper.Map<TransmissionDriveType>(driveType);
                await _context.TransmissionDriveTypes.AddAsync(entity);
                await _context.SaveChangesAsync();

                var vm = _mapper.Map<TransmissionDriveTypeViewModel>(entity);
                return OperationResult<TransmissionDriveTypeViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionDriveTypeViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionDriveTypeViewModel>> UpdateAsync(TransmissionDriveTypeUpdateDto dto)
        {
            try
            {
                var existing = await _context.TransmissionDriveTypes.FindAsync(dto.Id);
                if (existing == null)
                    return OperationResult<TransmissionDriveTypeViewModel>.Fail("Çekiş tipi bulunamadı");

                existing.Name = dto.Name;

                await _context.SaveChangesAsync();

                var vm = _mapper.Map<TransmissionDriveTypeViewModel>(existing);
                return OperationResult<TransmissionDriveTypeViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionDriveTypeViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TransmissionDriveTypes.FindAsync(id);
                if (entity == null)
                    return OperationResult<bool>.Fail("Çekiş tipi bulunamadı");

                _context.TransmissionDriveTypes.Remove(entity);
                await _context.SaveChangesAsync();

                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }
    }
}
