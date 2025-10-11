using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class VehiculoProfile : Profile
    {
        public VehiculoProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<VehiculoRequest, Vehiculo>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.MarcaVehiculoId, o => o.MapFrom(s => s.MarcaVehiculoId))
                .ForMember(d => d.ModeloVehiculoId, o => o.MapFrom(s => s.ModeloVehiculoId))
                .ForMember(d => d.Anio, o => o.MapFrom(s => s.Anio))
                .ForMember(d => d.Placa, o => o.MapFrom(s => s.Placa))
                .ForMember(d => d.Kilometraje, o => o.MapFrom(s => s.Kilometraje))
                .ForMember(d => d.ClienteId, o => o.MapFrom(s => s.ClienteId))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Vehiculo, VehiculoResponse>()
                .ForMember(d => d.VehiculoId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.MarcaVehiculoId, o => o.MapFrom(s => s.MarcaVehiculoId))
                .ForMember(d => d.ModeloVehiculoId, o => o.MapFrom(s => s.ModeloVehiculoId))
                .ForMember(d => d.Anio, o => o.MapFrom(s => s.Anio))
                .ForMember(d => d.Kilometraje, o => o.MapFrom(s => s.Kilometraje))
                .ForMember(d => d.Placa, o => o.MapFrom(s => s.Placa))
                .ForMember(d => d.ClienteId, o => o.MapFrom(s => s.ClienteId));
        }
    }
}


