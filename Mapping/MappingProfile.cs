using AutoMapper;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Models.Entities;
using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TransmissionStock, TransmissionStockViewModel>()
                .ForMember(d => d.TransmissionBrandName,
                    opt => opt.MapFrom(s => s.TransmissionBrand != null ? s.TransmissionBrand.Name : ""))

                .ForMember(d => d.VehicleBrandId,
                    opt => opt.MapFrom(s =>
                        s.VehicleBrandId ?? (s.VehicleModel != null ? (int?)s.VehicleModel.VehicleBrandId : null)
                    ))

                // ✅ null-safe
                .ForMember(d => d.VehicleBrandName,
                    opt => opt.MapFrom(s =>
                        s.VehicleBrand != null
                            ? s.VehicleBrand.Name
                            : (s.VehicleModel != null
                                ? (s.VehicleModel.VehicleBrand != null ? s.VehicleModel.VehicleBrand.Name : null)
                                : null)
                    ))

                .ForMember(d => d.DriveTypeId,
                    opt => opt.MapFrom(s => s.TransmissionDriveTypeId))
                .ForMember(d => d.DriveTypeName,
                    opt => opt.MapFrom(s => s.TransmissionDriveType != null ? s.TransmissionDriveType.Name : ""))

                .ForMember(d => d.VehicleModelName,
                    opt => opt.MapFrom(s => s.VehicleModel != null ? s.VehicleModel.Name : ""))

                .ForMember(d => d.TransmissionStatusName,
                    opt => opt.MapFrom(s => s.TransmissionStatus != null ? s.TransmissionStatus.Name : ""))

                // ✅ daha temiz shelf summary
                .ForMember(d => d.ShelfSummary, opt => opt.MapFrom(s =>
                    s.TransmissionStockLocations == null || !s.TransmissionStockLocations.Any()
                        ? ""
                        : string.Join(", ",
                            s.TransmissionStockLocations
                                .Where(tsl => tsl.Shelf != null)
                                .Select(tsl =>
                                    (tsl.Shelf.Warehouse != null && !string.IsNullOrWhiteSpace(tsl.Shelf.Warehouse.Name)
                                        ? tsl.Shelf.Warehouse.Name + "/"
                                        : "")
                                    + tsl.Shelf.ShelfCode
                                    + "(" + tsl.Quantity + ")"
                                )
                        )))


                .ForMember(d => d.TotalQuantity,
                    opt => opt.MapFrom(s => s.TransmissionStockLocations == null
                        ? 0
                        : s.TransmissionStockLocations.Sum(tsl => tsl.Quantity)))

                .ForMember(d => d.Shelves,
                    opt => opt.MapFrom(s => s.TransmissionStockLocations));

            CreateMap<TransmissionStockViewModel, TransmissionStock>()
                .ForMember(d => d.TransmissionBrand, opt => opt.Ignore())
                .ForMember(d => d.TransmissionDriveType, opt => opt.Ignore())
                .ForMember(d => d.VehicleModel, opt => opt.Ignore())
                .ForMember(d => d.VehicleBrand, opt => opt.Ignore())
                .ForMember(d => d.TransmissionStatus, opt => opt.Ignore())
                .ForMember(d => d.TransmissionStockLocations, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.Ignore());

            CreateMap<TransmissionStockLocation, ShelfViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Shelf.Id))
                .ForMember(d => d.ShelfCode, opt => opt.MapFrom(s => s.Shelf.ShelfCode))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForMember(d => d.WarehouseId, opt => opt.MapFrom(s => s.Shelf.WarehouseId))
                .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s =>
                    s.Shelf.Warehouse != null ? s.Shelf.Warehouse.Name : null));

            // ✅ ReverseMap güvenli hale getirildi
            CreateMap<Shelf, ShelfViewModel>()
                .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s =>
                    s.Warehouse != null ? s.Warehouse.Name : null))
                .ReverseMap()
                .ForMember(d => d.Warehouse, opt => opt.Ignore())
                .ForMember(d => d.TransmissionStockLocations, opt => opt.Ignore());

            CreateMap<TransmissionDriveType, TransmissionDriveTypeViewModel>().ReverseMap();
            CreateMap<TransmissionStatus, TransmissionStatusViewModel>().ReverseMap();
            CreateMap<VehicleBrand, VehicleBrandViewModel>().ReverseMap();
            CreateMap<VehicleModel, VehicleModelViewModel>().ReverseMap();

            CreateMap<VehicleBrandCreateDto, VehicleBrand>();
            CreateMap<VehicleBrandUpdateDto, VehicleBrand>();
            CreateMap<VehicleModelCreateDto, VehicleModel>();
            CreateMap<VehicleModelUpdateDto, VehicleModel>();

            CreateMap<Warehouse, WarehouseViewModel>().ReverseMap();
            CreateMap<WarehouseCreateDto, Warehouse>();
            CreateMap<WarehouseUpdateDto, Warehouse>();
        }
    }




}
