using AutoMapper;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile() 
        {
            CreateMap<Vehicle, Vehicle>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}