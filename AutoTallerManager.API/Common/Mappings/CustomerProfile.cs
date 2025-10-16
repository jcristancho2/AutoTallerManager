using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<CustomerRequest, Customer>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.CustomerTypeId, o => o.MapFrom(s => s.CustomerTypeId))
                .ForMember(d => d.AddressId, o => o.MapFrom(s => s.AddressId))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Customer, CustomerResponse>()
                .ForMember(d => d.CustomerTypeId, o => o.MapFrom(s => s.CustomerTypeId))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.AddressId, o => o.MapFrom(s => s.AddressId));
        }
    }
}


