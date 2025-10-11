using AutoMapper;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class OrdenServicioProfile : Profile
    {
        public OrdenServicioProfile() 
        {
            CreateMap<OrdenServicio, OrdenServicio>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}