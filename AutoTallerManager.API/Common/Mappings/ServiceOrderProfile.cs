using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.API.Common.Mappings
{
    public class ServiceOrderProfile : Profile
    {
        public ServiceOrderProfile()
        {
            // REQUEST -> DOMAIN
            CreateMap<ServiceOrderRequest, ServiceOrder>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.EntryDate, o => o.MapFrom(s => s.EntryDate))
                .ForMember(d => d.EstimatedDeliveryDate, o => o.MapFrom(s => s.E))
                .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.VehicleId))
                .ForMember(d => d.MechanicId, o => o.Ignore())
                .ForMember(d => d.ServiceTypeId, o => o.MapFrom(s => s.ServiceTypeId))
                .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<ServiceOrder, ServiceOrderResponse>()
                .ForMember(d => d.OrdenServicioId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d., o => o.MapFrom(s => s.))
                .ForMember(d => d.E, o => o.MapFrom(s => s.E))
                .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.VehicleId))
                .ForMember(d => d.ServiceTypeId, o => o.MapFrom(s => s.ServiceTypeId))
                .ForMember(d => d.StatusId, o => o.MapFrom(s => s.StatusId));
        }
    }
}


