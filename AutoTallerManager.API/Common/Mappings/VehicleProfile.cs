using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<VehicleRequest, Vehicle>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.branchId, o => o.MapFrom(s => s.branchId))
                .ForMember(d => d.ModelId, o => o.MapFrom(s => s.ModelId))
                .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                .ForMember(d => d.Plate, o => o.MapFrom(s => s.Plate))
                .ForMember(d => d.Mileage, o => o.MapFrom(s => s.Mileage))
                .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Vehicle, VehicleResponse>()
                .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.branchId, o => o.MapFrom(s => s.branchId))
                .ForMember(d => d.ModelId, o => o.MapFrom(s => s.ModelId))
                .ForMember(d => d.Year, o => o.MapFrom(s => s.Year))
                .ForMember(d => d.Mileage, o => o.MapFrom(s => s.Mileage))
                .ForMember(d => d.Plate, o => o.MapFrom(s => s.Plate))
                .ForMember(d => d., o => o.MapFrom(s => s.CustomerId));
        }
    }
}


