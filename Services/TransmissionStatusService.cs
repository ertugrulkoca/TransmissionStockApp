using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TransmissionStockApp.Data;
using TransmissionStockApp.Helpers;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class TransmissionStatusService : ITransmissionStatusService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;


        public TransmissionStatusService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<TransmissionStatusViewModel>>> GetAllAsync()
        {
            try
            {
                var list = await _context.TransmissionStatuses.OrderBy(s => s.Name).ToListAsync();
                var vmList = _mapper.Map<List<TransmissionStatusViewModel>>(list);
                return OperationResult<List<TransmissionStatusViewModel>>.Ok(vmList);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TransmissionStatusViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionStatusViewModel?>> GetByIdAsync(int id)
        {
            var status = await _context.TransmissionStatuses.FindAsync(id);
            var vm = _mapper.Map<TransmissionStatusViewModel>(status);

            return status != null
                ? OperationResult<TransmissionStatusViewModel?>.Ok(vm)
                : OperationResult<TransmissionStatusViewModel?>.Fail("Durum bulunamadı");
        }

        public async Task<OperationResult<TransmissionStatusViewModel>> CreateAsync(TransmissionStatusCreateDto dto)
        {
            try
            {
                var exists = await _context.TransmissionStatuses
                .AsNoTracking()
                .AnyAsync(w => w.Name == dto.Name);

                if (exists)
                    return OperationResult<TransmissionStatusViewModel>.Fail("Bu şanzıman durumu zaten mevcut.");

                var status = new TransmissionStatus { Name = dto.Name };
                _context.TransmissionStatuses.Add(status);
                await _context.SaveChangesAsync();
                var vm = _mapper.Map<TransmissionStatusViewModel>(status);
                return OperationResult<TransmissionStatusViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionStatusViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionStatusViewModel>> UpdateAsync(TransmissionStatusUpdateDto dto)
        {
            try
            {
                var status = new TransmissionStatus { Id = dto.Id, Name = dto.Name };
                var existing = await _context.TransmissionStatuses.FindAsync(status.Id);
                if (existing == null)
                    return OperationResult<TransmissionStatusViewModel>.Fail("Durum bulunamadı");

                existing.Name = status.Name;
                await _context.SaveChangesAsync();
                var vm = _mapper.Map<TransmissionStatusViewModel>(status);
                return OperationResult<TransmissionStatusViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionStatusViewModel>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            var entity = await _context.TransmissionStatuses.FindAsync(id);
            if (entity == null)
                return OperationResult<bool>.Fail("Kayıt bulunamadı.");

            try
            {
                _context.TransmissionStatuses.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyViolation(ex))
            {
                return OperationResult<bool>.Fail(
                    "Bu durum (status) bazı stok kayıtlarında kullanılıyor. " +
                    "Önce bu durumu kullanan stokları güncelleyin veya kaldırın.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

    }

}
