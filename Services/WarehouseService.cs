namespace TransmissionStockApp.Services
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using TransmissionStockApp.Data;
    using TransmissionStockApp.Models.DTOs;
    using TransmissionStockApp.Models.Entities;
    using TransmissionStockApp.Models.ViewModels;
    using TransmissionStockApp.Services.Interfaces;

    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WarehouseService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<WarehouseViewModel>>> GetAllAsync()
        {
            var warehouses = await _context.Warehouses
                .AsNoTracking()
                .OrderBy(w => w.Name)
                .ToListAsync();

            var viewModels = _mapper.Map<List<WarehouseViewModel>>(warehouses);
            return OperationResult<List<WarehouseViewModel>>.Ok(viewModels);
        }

        public async Task<OperationResult<WarehouseViewModel?>> GetByIdAsync(int id)
        {
            var warehouse = await _context.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouse == null)
                return OperationResult<WarehouseViewModel?>.Fail("Depo bulunamadı.");

            var viewModel = _mapper.Map<WarehouseViewModel>(warehouse);
            return OperationResult<WarehouseViewModel?>.Ok(viewModel);
        }

        public async Task<OperationResult<WarehouseViewModel>> CreateAsync(WarehouseCreateDto dto)
        {
            var name = dto.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<WarehouseViewModel>.Fail("Depo adı boş olamaz.");

            // Unique (case-insensitive collation var ama yine de erken kontrol iyi)
            var exists = await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Name == name);

            if (exists)
                return OperationResult<WarehouseViewModel>.Fail("Bu depo adı zaten mevcut.");

            var entity = new Warehouse { Name = name };
            _context.Warehouses.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return OperationResult<WarehouseViewModel>.Fail("Depo eklenemedi. Depo adı benzersiz olmalı.");
            }

            var viewModel = _mapper.Map<WarehouseViewModel>(entity);
            return OperationResult<WarehouseViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<WarehouseViewModel>> UpdateAsync(WarehouseUpdateDto dto)
        {
            var entity = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == dto.Id);
            if (entity == null)
                return OperationResult<WarehouseViewModel>.Fail("Depo bulunamadı.");

            var name = dto.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<WarehouseViewModel>.Fail("Depo adı boş olamaz.");

            var duplicate = await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id != dto.Id && w.Name == name);

            if (duplicate)
                return OperationResult<WarehouseViewModel>.Fail("Bu depo adı zaten mevcut.");

            entity.Name = name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return OperationResult<WarehouseViewModel>.Fail("Güncelleme başarısız. Depo adı benzersiz olmalı.");
            }

            var viewModel = _mapper.Map<WarehouseViewModel>(entity);
            return OperationResult<WarehouseViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            var entity = await _context.Warehouses.FindAsync(id);
            if (entity == null)
                return OperationResult<bool>.Fail("Depo bulunamadı.");

            try
            {
                _context.Warehouses.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException)
            {
                // Warehouse -> Shelf Restrict (raf varsa depo silinmez)
                return OperationResult<bool>.Fail("Bu depoda raflar varken silinemez. Önce rafları silin.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }
    }
}
