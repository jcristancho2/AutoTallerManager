using AutoMapper;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() 
        {
            // Mapeos básicos dentro de la capa Application
            CreateMap<Customer, Customer>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}