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

                // ÖNCE stoktaki marka, yoksa modelin markası
                .ForMember(d => d.VehicleBrandId,
                    opt => opt.MapFrom(s =>
                        s.VehicleBrandId ?? (s.VehicleModel != null ? (int?)s.VehicleModel.VehicleBrandId : null)
                    ))
                .ForMember(d => d.VehicleBrandName,
                    opt => opt.MapFrom(s =>
                        s.VehicleBrand != null
                            ? s.VehicleBrand.Name
                            : (s.VehicleModel != null ? s.VehicleModel.VehicleBrand.Name : null)
                    ))

                .ForMember(d => d.DriveTypeId,
                    opt => opt.MapFrom(s => s.TransmissionDriveTypeId))
                .ForMember(d => d.DriveTypeName,
                    opt => opt.MapFrom(s => s.TransmissionDriveType != null ? s.TransmissionDriveType.Name : ""))

                .ForMember(d => d.VehicleModelName,
                    opt => opt.MapFrom(s => s.VehicleModel != null ? s.VehicleModel.Name : ""))

                .ForMember(d => d.TransmissionStatusName,
                    opt => opt.MapFrom(s => s.TransmissionStatus != null ? s.TransmissionStatus.Name : ""))
                //.ForMember(d => d.ShelfSummary,
                //    opt => opt.MapFrom(s => string.Join(", ",
                //        s.TransmissionStockLocations.Select(tsl => $"{tsl.StockLocation.ShelfCode}({tsl.Quantity})"))))
                .ForMember(d => d.ShelfSummary, opt => opt.MapFrom(s =>
                    s.TransmissionStockLocations == null || !s.TransmissionStockLocations.Any()
                      ? ""
                      : string.Join(", ",
                          s.TransmissionStockLocations
                            .Where(tsl => tsl.StockLocation != null)
                            .Select(tsl => $"{tsl.StockLocation.ShelfCode}({tsl.Quantity})"))))

                .ForMember(d => d.TotalQuantity,
                    opt => opt.MapFrom(s => s.TransmissionStockLocations.Sum(tsl => tsl.Quantity)))

                .ForMember(d => d.StockLocations,
                    opt => opt.MapFrom(s => s.TransmissionStockLocations));

            CreateMap<TransmissionStockViewModel, TransmissionStock>()
                .ForMember(d => d.TransmissionBrand, opt => opt.Ignore())
                .ForMember(d => d.TransmissionDriveType, opt => opt.Ignore())
                .ForMember(d => d.VehicleModel, opt => opt.Ignore())
                .ForMember(d => d.VehicleBrand, opt => opt.Ignore())
                .ForMember(d => d.TransmissionStatus, opt => opt.Ignore())
                .ForMember(d => d.TransmissionStockLocations, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.Ignore());

            CreateMap<TransmissionStockLocation, StockLocationViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.StockLocation.Id))
                .ForMember(d => d.ShelfCode, opt => opt.MapFrom(s => s.StockLocation.ShelfCode))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity));

            CreateMap<StockLocation, StockLocationViewModel>().ReverseMap();
            CreateMap<TransmissionDriveType, TransmissionDriveTypeViewModel>().ReverseMap();
            CreateMap<TransmissionStatus, TransmissionStatusViewModel>().ReverseMap();
            CreateMap<VehicleBrand, VehicleBrandViewModel>().ReverseMap();
            CreateMap<VehicleModel, VehicleModelViewModel>().ReverseMap();
            CreateMap<VehicleBrandCreateDto, VehicleBrand>();
            CreateMap<VehicleBrandUpdateDto, VehicleBrand>();
            CreateMap<VehicleModelCreateDto, VehicleModel>();
            CreateMap<VehicleModelUpdateDto, VehicleModel>();
        }
    }



}
