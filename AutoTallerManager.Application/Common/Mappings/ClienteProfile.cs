using AutoMapper;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile() 
        {
            // Mapeos básicos dentro de la capa Application
            CreateMap<Cliente, Cliente>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}