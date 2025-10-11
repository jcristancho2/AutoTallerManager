using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class RepuestoProfile : Profile
    {
        public RepuestoProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<RepuestoRequest, Repuesto>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Codigo, o => o.MapFrom(s => s.Codigo))
                .ForMember(d => d.NombreRepu, o => o.MapFrom(s => s.NombreRepu))
                .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => s.PrecioUnitario))
                .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stock))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Repuesto, RepuestoResponse>()
                .ForMember(d => d.Codigo, o => o.MapFrom(s => s.Codigo))
                .ForMember(d => d.RepuestoId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.NombreRepu, o => o.MapFrom(s => s.NombreRepu))
                .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => s.PrecioUnitario))
                .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stock));
        }
    }
}


