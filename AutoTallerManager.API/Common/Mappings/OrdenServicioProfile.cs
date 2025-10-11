using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class OrdenServicioProfile : Profile
    {
        public OrdenServicioProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<OrdenServicioRequest, OrdenServicio>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.FechaIngreso, o => o.MapFrom(s => s.FechaIngreso))
                .ForMember(d => d.FechaEstimadaEntrega, o => o.MapFrom(s => s.FechaEstimadaEntrega))
                .ForMember(d => d.VehiculoId, o => o.MapFrom(s => s.VehiculoId))
                .ForMember(d => d.MecanicoId, o => o.Ignore())
                .ForMember(d => d.TipoServId, o => o.MapFrom(s => s.TipoServId))
                .ForMember(d => d.EstadoId, o => o.MapFrom(s => s.EstadoId))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<OrdenServicio, OrdenServicioResponse>()
                .ForMember(d => d.OrdenServicioId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FechaIngreso, o => o.MapFrom(s => s.FechaIngreso))
                .ForMember(d => d.FechaEstimadaEntrega, o => o.MapFrom(s => s.FechaEstimadaEntrega))
                .ForMember(d => d.VehiculoId, o => o.MapFrom(s => s.VehiculoId))
                .ForMember(d => d.TipoServId, o => o.MapFrom(s => s.TipoServId))
                .ForMember(d => d.EstadoId, o => o.MapFrom(s => s.EstadoId));
        }
    }
}


