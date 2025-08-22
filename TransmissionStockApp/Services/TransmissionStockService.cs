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

        public TransmissionStockService(ITransmissionStockRepository repository, AppDbContext context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }
        public async Task<OperationResult<List<TransmissionStockViewModel>>> GetAllAsync()
        {
            try
            {
                var stocks = await _repository.GetAllWithRelationsAsync(); // Navigation propertylerle birlikte
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
            }

            VehicleModel? vehicleModel = null;
            if (dto.VehicleModelId.HasValue)
            {
                vehicleModel = await _context.VehicleModels.FindAsync(dto.VehicleModelId.Value);
            }

            // Aynı kombinasyon var mı?
            var exists = await _context.TransmissionStocks.AnyAsync(ts =>
                ts.TransmissionBrandId == dto.TransmissionBrandId &&
                ts.SparePartNo == dto.SparePartNo &&
                ts.TransmissionStatusId == dto.TransmissionStatusId);

            if (exists)
            {
                return OperationResult<TransmissionStockViewModel>.Fail(
                    "Bu marka + parça no + durum kombinasyonu zaten mevcut. Lütfen 'Mevcut kaydı aç ve düzenle' ile güncelleyin.");
            }

            // Yeni ürün ekleniyor
            var newStock = new TransmissionStock
            {
                SparePartNo = dto.SparePartNo,
                TransmissionBrandId = brand.Id,
                TransmissionCode = dto.TransmissionCode,
                TransmissionNumber = dto.TransmissionNumber,
                Year = dto.Year,
                VehicleModelId = vehicleModel?.Id,
                TransmissionStatusId = status.Id,
                TransmissionDriveTypeId = driveType?.Id,
                Description = dto.Description,
                TransmissionStockLocations = new List<TransmissionStockLocation>()
            };

            var normalizedLocations = NormalizeStockLocations(dto.StockLocations);

            foreach (var slDto in normalizedLocations)
            {
                var stockLocation = await _context.StockLocations
                    .FirstOrDefaultAsync(s => s.ShelfCode == slDto.ShelfCode);

                if (stockLocation == null)
                {
                    return OperationResult<TransmissionStockViewModel>.Fail(
                        $"Stok lokasyonu bulunamadı: {slDto.ShelfCode}");
                }

                newStock.TransmissionStockLocations.Add(new TransmissionStockLocation
                {
                    StockLocationId = stockLocation.Id,
                    Quantity = slDto.Quantity
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
                    "Bu marka + parça no + durum kombinasyonu zaten mevcut."
                );
            }

            var created = await _context.TransmissionStocks
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionDriveType)
                .Include(ts => ts.VehicleBrand)
                .Include(ts => ts.VehicleModel)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.StockLocation)
                .FirstOrDefaultAsync(ts => ts.Id == newStock.Id);

            var vm = _mapper.Map<TransmissionStockViewModel>(created);
            return OperationResult<TransmissionStockViewModel>.Ok(vm);
        }

        public async Task<OperationResult<TransmissionStockViewModel>> UpdateAsync(int id, TransmissionStockUpdateDto dto)
        {
            try
            {
                var existing = await _context.TransmissionStocks
                    .Include(ts => ts.TransmissionStockLocations)
                        .ThenInclude(tsl => tsl.StockLocation)
                    .FirstOrDefaultAsync(ts => ts.Id == id);

                if (existing == null)
                    return OperationResult<TransmissionStockViewModel>.Fail("Kayıt bulunamadı.");

                // ÇAKIŞMA KONTROLÜ (aynı marka + parça no + durum başka bir kayıtta var mı?)
                var conflict = await _context.TransmissionStocks
                    .AnyAsync(ts =>
                        ts.Id != id &&
                        ts.TransmissionBrandId == dto.TransmissionBrandId &&
                        ts.SparePartNo == dto.SparePartNo &&
                        ts.TransmissionStatusId == dto.TransmissionStatusId);

                if (conflict)
                    return OperationResult<TransmissionStockViewModel>.Fail(
                        "Aynı yedek parça numarası, marka ve durum kombinasyonuna sahip başka bir ürün zaten mevcut.");

                // İlişkili entity'leri doğrula
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
                }

                // Marka tutarlılığı:
                // - Model varsa -> marka daima modelden gelir
                // - Model yoksa -> dto.VehicleBrandId varsa onu kullanırız (yoksa null kalır)
                int? vehicleBrandIdToUse = null;
                if (vehicleModel != null)
                {
                    vehicleBrandIdToUse = vehicleModel.VehicleBrandId;
                }
                else
                {
                    vehicleBrandIdToUse = dto.VehicleBrandId; // null olabilir
                }

                // Ana alanları güncelle
                existing.SparePartNo = dto.SparePartNo;
                existing.TransmissionBrandId = brand.Id;
                existing.TransmissionCode = dto.TransmissionCode;
                existing.TransmissionNumber = dto.TransmissionNumber;
                existing.Year = dto.Year;

                existing.VehicleBrandId = vehicleBrandIdToUse;   // <-- YENİ
                existing.VehicleModelId = vehicleModel?.Id;

                existing.TransmissionStatusId = status.Id;
                existing.TransmissionDriveTypeId = driveType?.Id;
                existing.Description = dto.Description;

                // --- LOKASYONLAR ---
                // 1) Mevcut bağları kaldır (TSL)
                _context.TransmissionStockLocations.RemoveRange(existing.TransmissionStockLocations);
                await _context.SaveChangesAsync(); // tracking temiz

                existing.TransmissionStockLocations.Clear();

                // 2) Gelen lokasyonları normalize et (aynı rafları birleştir)
                var normalizedLocations = NormalizeStockLocations(dto.StockLocations);

                // 3) Yeni lokasyonları ekle (StockLocationId kullan)
                foreach (var slDto in normalizedLocations)
                {
                    var stockLocation = await _context.StockLocations
                        .FirstOrDefaultAsync(s => s.ShelfCode == slDto.ShelfCode);

                    if (stockLocation == null)
                        return OperationResult<TransmissionStockViewModel>.Fail(
                            $"Stok lokasyonu bulunamadı: ShelfCode={slDto.ShelfCode}");

                    existing.TransmissionStockLocations.Add(new TransmissionStockLocation
                    {
                        StockLocationId = stockLocation.Id,
                        Quantity = slDto.Quantity
                    });
                }

                _context.TransmissionStocks.Update(existing);

                try
                {
                    var saved = await _context.SaveChangesAsync();
                    if (saved <= 0)
                        return OperationResult<TransmissionStockViewModel>.Fail("Güncelleme başarısız.");
                }
                catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueViolation(ex))
                {
                    // (BrandId, SparePartNo, StatusId) benzersizlik ihlali
                    return OperationResult<TransmissionStockViewModel>.Fail(
                        "Bu marka + parça no + durum kombinasyonu zaten mevcut.");
                }

                // Güncellenmiş veriyi çek ve dön
                var updatedEntity = await _context.TransmissionStocks
                    .Include(ts => ts.TransmissionBrand)
                    .Include(ts => ts.TransmissionStatus)
                    .Include(ts => ts.TransmissionDriveType)
                    .Include(ts => ts.VehicleBrand)
                    .Include(ts => ts.VehicleModel)
                    .Include(ts => ts.TransmissionStockLocations)
                        .ThenInclude(tsl => tsl.StockLocation)
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
            catch (DbUpdateException ex)
            {
                // Teorik olarak burada FK ihlali beklemiyoruz;
                // ama provider kaynaklı başka bir kısıt patlarsa buraya düşer.
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
                .Include(ts => ts.VehicleModel)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.StockLocation)
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


        public async Task<OperationResult<DuplicateCheckViewModel>> CheckDuplicateAsync(int transmissionBrandId, string sparePartNo, int transmissionStatusId)
        {
            var existing = await _context.TransmissionStocks
                .Include(ts => ts.TransmissionBrand)
                .Include(ts => ts.TransmissionStatus)
                .Include(ts => ts.TransmissionStockLocations)
                    .ThenInclude(tsl => tsl.StockLocation)
                .FirstOrDefaultAsync(ts =>
                    ts.TransmissionBrandId == transmissionBrandId &&
                    ts.SparePartNo == sparePartNo &&
                    ts.TransmissionStatusId == transmissionStatusId);

            if (existing == null)
                return OperationResult<DuplicateCheckViewModel>.Ok(new DuplicateCheckViewModel { Exists = false });

            var totalQty = existing.TransmissionStockLocations.Sum(x => x.Quantity);
            var shelfSummary = string.Join(", ",
                existing.TransmissionStockLocations
                    .OrderBy(x => x.StockLocation.ShelfCode)
                    .Select(x => $"{x.StockLocation.ShelfCode}({x.Quantity})"));

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

        private static List<StockLocationQuantityDto> NormalizeStockLocations(IEnumerable<StockLocationQuantityDto> items)
        {
            if (items == null) return new List<StockLocationQuantityDto>();

            // Negatif/sıfırları at, boş raf kodlarını at, trimle, case-insensitive grupla
            return items
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.ShelfCode) && x.Quantity > 0)
                .GroupBy(x => x.ShelfCode.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => new StockLocationQuantityDto
                {
                    ShelfCode = g.Key,                       // g.Key zaten trim’lenmiş
                    Quantity = g.Sum(x => x.Quantity)        // aynı raf için miktarları topla
                })
                .ToList();
        }

    }

}
