using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class ShelfService : IShelfService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShelfService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<ShelfViewModel>>> GetAllAsync()
        {
            // WarehouseName göstereceksen include şart
            var shelves = await _context.Shelves
                .AsNoTracking()
                .Include(s => s.Warehouse)
                .OrderBy(s => s.Warehouse.Name)
                .ThenBy(s => s.ShelfCode)
                .ToListAsync();

            var viewModels = _mapper.Map<List<ShelfViewModel>>(shelves);
            return OperationResult<List<ShelfViewModel>>.Ok(viewModels);
        }

        public async Task<OperationResult<ShelfViewModel?>> GetByIdAsync(int id)
        {
            var shelf = await _context.Shelves
                .AsNoTracking()
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelf == null)
                return OperationResult<ShelfViewModel?>.Fail("Raf bulunamadı.");

            var viewModel = _mapper.Map<ShelfViewModel>(shelf);
            return OperationResult<ShelfViewModel?>.Ok(viewModel);
        }

        public async Task<OperationResult<ShelfViewModel>> CreateAsync(ShelfCreateDto dto)
        {
            // Warehouse var mı?
            var warehouseExists = await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id == dto.WarehouseId);

            if (!warehouseExists)
                return OperationResult<ShelfViewModel>.Fail("Depo bulunamadı.");

            // Aynı depoda aynı raf kodu var mı? (iş kuralın)
            var exists = await _context.Shelves
                .AsNoTracking()
                .AnyAsync(s => s.WarehouseId == dto.WarehouseId && s.ShelfCode == dto.ShelfCode);

            if (exists)
                return OperationResult<ShelfViewModel>.Fail("Bu depoda aynı raf kodu zaten mevcut.");

            var shelf = new Shelf
            {
                WarehouseId = dto.WarehouseId,
                ShelfCode = dto.ShelfCode.Trim()
            };

            _context.Shelves.Add(shelf);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Unique index çakışması vs.
                return OperationResult<ShelfViewModel>.Fail("Raf eklenemedi. (Depo + Raf kodu) benzersiz olmalı.");
            }

            // WarehouseName için tekrar include ile çek
            var created = await _context.Shelves
                .AsNoTracking()
                .Include(s => s.Warehouse)
                .FirstAsync(s => s.Id == shelf.Id);

            var viewModel = _mapper.Map<ShelfViewModel>(created);
            return OperationResult<ShelfViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<ShelfViewModel>> UpdateAsync(ShelfUpdateDto dto)
        {
            var existing = await _context.Shelves
                .Include(s => s.Warehouse) // mapping için
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (existing == null)
                return OperationResult<ShelfViewModel>.Fail("Raf bulunamadı.");

            // Warehouse değiştirilecekse yeni depo var mı?
            var warehouseExists = await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id == dto.WarehouseId);

            if (!warehouseExists)
                return OperationResult<ShelfViewModel>.Fail("Depo bulunamadı.");

            // Aynı depoda aynı raf kodu başka kayıtta var mı?
            var duplicate = await _context.Shelves
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Id != dto.Id &&
                    s.WarehouseId == dto.WarehouseId &&
                    s.ShelfCode == dto.ShelfCode);

            if (duplicate)
                return OperationResult<ShelfViewModel>.Fail("Bu depoda aynı raf kodu zaten mevcut.");

            existing.WarehouseId = dto.WarehouseId;
            existing.ShelfCode = dto.ShelfCode.Trim();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return OperationResult<ShelfViewModel>.Fail("Güncelleme başarısız. (Depo + Raf kodu) benzersiz olmalı.");
            }

            // Warehouse navigation değişmiş olabilir, güncelini yükle
            await _context.Entry(existing).Reference(x => x.Warehouse).LoadAsync();

            var viewModel = _mapper.Map<ShelfViewModel>(existing);
            return OperationResult<ShelfViewModel>.Ok(viewModel);
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            var entity = await _context.Shelves.FindAsync(id);
            if (entity == null)
                return OperationResult<bool>.Fail("Raf bulunamadı.");

            try
            {
                _context.Shelves.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException)
            {
                // FK Restrict: TransmissionStockLocation bağlıysa silinmez
                return OperationResult<bool>.Fail("Bu rafta bağlı stok varken silinemez. Önce stok bağlantılarını kaldırın.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<List<ShelfViewModel>>> GetByWarehouseAsync(int warehouseId)
        {
            var exists = await _context.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id == warehouseId);

            if (!exists)
                return OperationResult<List<ShelfViewModel>>.Fail("Depo bulunamadı.");

            var shelves = await _context.Shelves
                .AsNoTracking()
                .Include(s => s.Warehouse)
                .Where(s => s.WarehouseId == warehouseId)
                .OrderBy(s => s.ShelfCode)
                .ToListAsync();

            var vms = _mapper.Map<List<ShelfViewModel>>(shelves);
            return OperationResult<List<ShelfViewModel>>.Ok(vms);
        }

    }
}
