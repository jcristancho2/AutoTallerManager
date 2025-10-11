using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<ClienteRequest, Cliente>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.NombreCompleto, o => o.MapFrom(s => s.NombreCompleto))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Telefono))
                .ForMember(d => d.TipoCliente_Id, o => o.MapFrom(s => s.TipoCliente_Id))
                .ForMember(d => d.Direccion_Id, o => o.MapFrom(s => s.Direccion_Id))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Cliente, ClienteResponse>()
                .ForMember(d => d.TipoCliente_Id, o => o.MapFrom(s => s.TipoCliente_Id))
                .ForMember(d => d.NombreCompleto, o => o.MapFrom(s => s.NombreCompleto))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Telefono))
                .ForMember(d => d.Direccion_Id, o => o.MapFrom(s => s.Direccion_Id));
        }
    }
}


