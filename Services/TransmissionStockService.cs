using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Data;
using TransmissionStockApp.Helpers;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;
using TransmissionStockApp.Repositories;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Services
{
    public class TransmissionStockService : ITransmissionStockService
    {
        private readonly ITransmissionStockRepository _repository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransmissionStockService(
            ITransmissionStockRepository repository,
            AppDbContext context,
            IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<TransmissionStockViewModel>>> GetAllAsync()
        {
            try
            {
                var stocks = await _repository.GetAllWithRelationsAsync();
                var vmList = _mapper.Map<List<TransmissionStockViewModel>>(stocks);
                return OperationResult<List<TransmissionStockViewModel>>.Ok(vmList);
            }
            catch (Exception ex)
            {
                return OperationResult<List<TransmissionStockViewModel>>.Fail(ex.Message);
            }
        }

        public async Task<OperationResult<TransmissionStockViewModel>> GetByIdAsync(int id)
        {
            var stock = await _repository.GetByIdWithRelationsAsync(id);
            if (stock == null)
                return OperationResult<TransmissionStockViewModel>.Fail("Kayıt bulunamadı");

            var vm = _mapper.Map<TransmissionStockViewModel>(stock);
            return OperationResult<TransmissionStockViewModel>.Ok(vm);
        }

        public async Task<OperationResult<TransmissionStockViewModel>> CreateAsync(TransmissionStockCreateDto dto)
        {
            // 0) Depo/Raf zorunlu kontrolü (ham DTO)
            if (dto.Shelves == null || dto.Shelves.Count == 0)
                return OperationResult<TransmissionStockViewModel>.Fail("Depo/Raf bilgisi zorunludur. Lütfen en az 1 raf satırı ekleyin.");

            // Zorunlu ilişkiler
            var brand = await _context.TransmissionBrands.FindAsync(dto.TransmissionBrandId);
            if (brand == null)
                return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz şanzıman markası.");

            var status = await _context.TransmissionStatuses.FindAsync(dto.TransmissionStatusId);
            if (status == null)
                return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz durum.");

            // Opsiyonel ilişkiler
            TransmissionDriveType? driveType = null;
            if (dto.DriveTypeId.HasValue)
            {
                driveType = await _context.TransmissionDriveTypes
                    .FirstOrDefaultAsync(d => d.Id == dto.DriveTypeId.Value);

                if (driveType == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz çekiş tipi.");
            }

            VehicleModel? vehicleModel = null;

            if (dto.VehicleModelId.HasValue)
            {
                vehicleModel = await _context.VehicleModels
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == dto.VehicleModelId.Value);

                if (vehicleModel == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz araç modeli.");

                // Eğer ayrıca araç markası da gönderildiyse tutarlılık kontrolü yap
                if (dto.VehicleBrandId.HasValue && vehicleModel.VehicleBrandId != dto.VehicleBrandId.Value)
                    return OperationResult<TransmissionStockViewModel>.Fail("Seçilen araç modeli, seçilen araç markasına ait değil.");
            }

            // Marka tutarlılığı (model varsa modelin markası esas)
            int? vehicleBrandIdToUse = vehicleModel != null
                ? vehicleModel.VehicleBrandId
                : dto.VehicleBrandId;


            // Unique kombinasyon kontrolü
            var exists = await _context.TransmissionStocks.AnyAsync(ts =>
                ts.TransmissionBrandId == dto.TransmissionBrandId &&
                ts.SparePartNo == dto.SparePartNo &&
                ts.TransmissionStatusId == dto.TransmissionStatusId);

            if (exists)
            {
                return OperationResult<TransmissionStockViewModel>.Fail(
                    "Bu marka + parça no + durum kombinasyonu zaten mevcut. Lütfen 'Mevcut kaydı aç ve düzenle' ile güncelleyin.");
            }

            var newStock = new TransmissionStock
            {
                SparePartNo = dto.SparePartNo,
                TransmissionBrandId = brand.Id,
                TransmissionCode = dto.TransmissionCode,
                TransmissionNumber = dto.TransmissionNumber,
                Year = dto.Year,

                VehicleBrandId = vehicleBrandIdToUse,
                VehicleModelId = vehicleModel?.Id,

                TransmissionStatusId = status.Id,
                TransmissionDriveTypeId = driveType?.Id,

                Description = dto.Description,
                TransmissionStockLocations = new List<TransmissionStockLocation>()
            };

            // --- LOKASYONLAR (ShelfId bazlı) ---
            var normalizedShelves = NormalizeShelves(dto.Shelves);

            // 1) Normalize sonrası da zorunlu (ör. aynı raflar birleşip 0 kalmasın gibi)
            if (normalizedShelves.Count == 0)
                return OperationResult<TransmissionStockViewModel>.Fail("Depo/Raf bilgisi zorunludur. Lütfen en az 1 geçerli raf satırı ekleyin.");

            // 2) Quantity kontrolü (istersen bu kuralı kaldırırız ama stok için öneririm)
            if (normalizedShelves.Any(x => x.Quantity <= 0))
                return OperationResult<TransmissionStockViewModel>.Fail("Raf adet değeri 0'dan büyük olmalıdır.");

            // Raf id doğrulama (tek sorgu)
            var shelfIds = normalizedShelves.Select(x => x.ShelfId).ToList();

            var existingShelfIds = await _context.Shelves
                .Where(s => shelfIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            if (existingShelfIds.Count != shelfIds.Count)
                return OperationResult<TransmissionStockViewModel>.Fail("Seçilen raflardan biri veya birkaçı bulunamadı.");

            foreach (var shDto in normalizedShelves)
            {
                newStock.TransmissionStockLocations.Add(new TransmissionStockLocation
                {
                    ShelfId = shDto.ShelfId,
                    Quantity = shDto.Quantity
                });
            }

            try
            {
                _context.TransmissionStocks.Add(newStock);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
            {
                return OperationResult<TransmissionStockViewModel>.Fail(
                    "Bu marka + parça no + durum kombinasyonu zaten mevcut.");
            }

            // Dönüş için ilişkilerle tekrar çek (Warehouse dahil)
            var created = await _context.TransmissionStocks
                .AsNoTracking()
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(vm => vm.VehicleBrand)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.Shelf)
                        .ThenInclude(s => s.Warehouse)
                .FirstOrDefaultAsync(ts => ts.Id == newStock.Id);

            var vm = _mapper.Map<TransmissionStockViewModel>(created!);
            return OperationResult<TransmissionStockViewModel>.Ok(vm);
        }


        public async Task<OperationResult<TransmissionStockViewModel>> UpdateAsync(int id, TransmissionStockUpdateDto dto)
        {
            try
            {
                // 0) Depo/Raf zorunlu kontrolü
                if (dto.Shelves == null || dto.Shelves.Count == 0)
                    return OperationResult<TransmissionStockViewModel>.Fail("Depo/Raf bilgisi zorunludur. Lütfen en az 1 raf satırı ekleyin.");

                // Tracking + Locations include
                var existing = await _context.TransmissionStocks
                    .Include(ts => ts.TransmissionStockLocations)
                    .FirstOrDefaultAsync(ts => ts.Id == id);

                if (existing == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Kayıt bulunamadı.");

                // Çakışma kontrolü
                var conflict = await _context.TransmissionStocks
                    .AnyAsync(ts =>
                        ts.Id != id &&
                        ts.TransmissionBrandId == dto.TransmissionBrandId &&
                        ts.SparePartNo == dto.SparePartNo &&
                        ts.TransmissionStatusId == dto.TransmissionStatusId);

                if (conflict)
                    return OperationResult<TransmissionStockViewModel>.Fail(
                        "Aynı yedek parça numarası, marka ve durum kombinasyonuna sahip başka bir ürün zaten mevcut.");

                // İlişkileri doğrula
                var brand = await _context.TransmissionBrands.FindAsync(dto.TransmissionBrandId);
                if (brand == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz şanzıman markası.");

                var status = await _context.TransmissionStatuses.FindAsync(dto.TransmissionStatusId);
                if (status == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz durum.");

                TransmissionDriveType? driveType = null;
                if (dto.DriveTypeId.HasValue)
                {
                    driveType = await _context.TransmissionDriveTypes
                        .FirstOrDefaultAsync(d => d.Id == dto.DriveTypeId.Value);

                    if (driveType == null)
                        return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz çekiş tipi.");
                }

                VehicleModel? vehicleModel = null;

                if (dto.VehicleModelId.HasValue)
                {
                    vehicleModel = await _context.VehicleModels
                        .AsNoTracking()
                        .FirstOrDefaultAsync(v => v.Id == dto.VehicleModelId.Value);

                    if (vehicleModel == null)
                        return OperationResult<TransmissionStockViewModel>.Fail("Geçersiz araç modeli.");

                    // Eğer ayrıca araç markası da gönderildiyse tutarlılık kontrolü yap
                    if (dto.VehicleBrandId.HasValue && vehicleModel.VehicleBrandId != dto.VehicleBrandId.Value)
                        return OperationResult<TransmissionStockViewModel>.Fail("Seçilen araç modeli, seçilen araç markasına ait değil.");
                }

                // Marka tutarlılığı (model varsa modelin markası esas)
                int? vehicleBrandIdToUse = vehicleModel != null
                    ? vehicleModel.VehicleBrandId
                    : dto.VehicleBrandId;

                // Ana alanları güncelle
                existing.SparePartNo = dto.SparePartNo;
                existing.TransmissionBrandId = brand.Id;
                existing.TransmissionCode = dto.TransmissionCode;
                existing.TransmissionNumber = dto.TransmissionNumber;
                existing.Year = dto.Year;

                existing.VehicleBrandId = vehicleBrandIdToUse;
                existing.VehicleModelId = vehicleModel?.Id;

                existing.TransmissionStatusId = status.Id;
                existing.TransmissionDriveTypeId = driveType?.Id;
                existing.Description = dto.Description;

                // --- LOKASYONLAR ---
                var normalizedShelves = NormalizeShelves(dto.Shelves);

                if (normalizedShelves.Count == 0)
                    return OperationResult<TransmissionStockViewModel>.Fail("Depo/Raf bilgisi zorunludur. Lütfen en az 1 geçerli raf satırı ekleyin.");

                if (normalizedShelves.Any(x => x.Quantity <= 0))
                    return OperationResult<TransmissionStockViewModel>.Fail("Raf adet değeri 0'dan büyük olmalıdır.");

                // Eski bağları tek seferde temizle (orphan delete / cascade)
                existing.TransmissionStockLocations.Clear();

                // Raf id doğrulama (tek sorgu)
                var shelfIds = normalizedShelves.Select(x => x.ShelfId).ToList();

                var existingShelfIds = await _context.Shelves
                    .Where(s => shelfIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

                if (existingShelfIds.Count != shelfIds.Count)
                    return OperationResult<TransmissionStockViewModel>.Fail("Seçilen raflardan biri veya birkaçı bulunamadı.");

                foreach (var shDto in normalizedShelves)
                {
                    existing.TransmissionStockLocations.Add(new TransmissionStockLocation
                    {
                        ShelfId = shDto.ShelfId,
                        Quantity = shDto.Quantity
                    });
                }

                try
                {
                    var saved = await _context.SaveChangesAsync();
                    if (saved <= 0)
                        return OperationResult<TransmissionStockViewModel>.Fail("Güncelleme başarısız.");
                }
                catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
                {
                    return OperationResult<TransmissionStockViewModel>.Fail(
                        "Bu marka + parça no + durum kombinasyonu zaten mevcut.");
                }

                // Güncel entity’yi ilişkilerle çek (Warehouse dahil)
                var updatedEntity = await _context.TransmissionStocks
                    .AsNoTracking()
                    .Include(ts => ts.TransmissionBrand)
                    .Include(ts => ts.TransmissionStatus)
                    .Include(ts => ts.TransmissionDriveType)
                    .Include(ts => ts.VehicleBrand)
                    .Include(ts => ts.VehicleModel)
                        .ThenInclude(vm => vm.VehicleBrand)
                    .Include(ts => ts.TransmissionStockLocations)
                        .ThenInclude(tsl => tsl.Shelf)
                            .ThenInclude(s => s.Warehouse)
                    .FirstOrDefaultAsync(ts => ts.Id == id);

                var vm = _mapper.Map<TransmissionStockViewModel>(updatedEntity!);
                return OperationResult<TransmissionStockViewModel>.Ok(vm);
            }
            catch (Exception ex)
            {
                return OperationResult<TransmissionStockViewModel>.Fail($"Hata: {ex.Message}");
            }
        }


        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var affected = await _context.TransmissionStocks
                    .Where(ts => ts.Id == id)
                    .ExecuteDeleteAsync();

                if (affected == 0)
                    return OperationResult<bool>.Fail("Kayıt bulunamadı veya zaten silinmiş.");

                return OperationResult<bool>.Ok(true);
            }
            catch (DbUpdateException)
            {
                return OperationResult<bool>.Fail("Silme işlemi gerçekleştirilemedi.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<PagedResult<TransmissionStockViewModel>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.TransmissionStocks
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                    .ThenInclude(vm => vm.VehicleBrand)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.Shelf)
                        .ThenInclude(s => s.Warehouse)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TransmissionStockViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TransmissionStockViewModel>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            };
        }

        public async Task<OperationResult<DuplicateCheckViewModel>> CheckDuplicateAsync(
            int transmissionBrandId,
            string sparePartNo,
            int transmissionStatusId)
        {
            var existing = await _context.TransmissionStocks
                .AsNoTracking()
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.Shelf)
                        .ThenInclude(s => s.Warehouse)
                .FirstOrDefaultAsync(ts =>
                    ts.TransmissionBrandId == transmissionBrandId &&
                    ts.SparePartNo == sparePartNo &&
                    ts.TransmissionStatusId == transmissionStatusId);

            if (existing == null)
                return OperationResult<DuplicateCheckViewModel>.Ok(new DuplicateCheckViewModel { Exists = false });

            var totalQty = existing.TransmissionStockLocations.Sum(x => x.Quantity);

            // ✅ Warehouse/Shelf ile özet
            var shelfSummary = string.Join(", ",
                existing.TransmissionStockLocations
                    .OrderBy(x => x.Shelf.Warehouse.Name)
                    .ThenBy(x => x.Shelf.ShelfCode)
                    .Select(x => $"{x.Shelf.Warehouse.Name}/{x.Shelf.ShelfCode}({x.Quantity})"));

            return OperationResult<DuplicateCheckViewModel>.Ok(new DuplicateCheckViewModel
            {
                Exists = true,
                ExistingId = existing.Id,
                TransmissionBrandName = existing.TransmissionBrand.Name,
                TransmissionStatusName = existing.TransmissionStatus.Name,
                SparePartNo = existing.SparePartNo,
                TotalQuantity = totalQty,
                ShelfSummary = shelfSummary
            });
        }

        private static List<ShelfIdQuantityDto> NormalizeShelves(IEnumerable<ShelfIdQuantityDto>? items)
        {
            if (items == null) return new();

            return items
                .Where(x => x != null && x.ShelfId > 0 && x.Quantity > 0)
                .GroupBy(x => x.ShelfId)
                .Select(g => new ShelfIdQuantityDto
                {
                    ShelfId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();
        }
    }
}
