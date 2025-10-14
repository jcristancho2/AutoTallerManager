using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class SparePartsProfile : Profile
    {
        public SparePartsProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<SparePartsRequest, SpareParts>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice))
                .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stock))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Repuesto, RepuestoResponse>()
                .ForMember(d => d.SparePartsId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice))
                .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stock));
        }
    }
}


