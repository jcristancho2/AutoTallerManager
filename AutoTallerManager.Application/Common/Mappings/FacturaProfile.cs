using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoTallerManager.API.DTOs.Request;
using AutoTallerManager.API.DTOs.Response;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class FacturaProfile : Profile
    {
        public FacturaProfile()
        {
             /// REQUEST -> DOMAIN
        CreateMap<FacturaRequest, Factura>()
            .ForMember(d => d.Id, o => o.Ignore()) // lo genera DB/app
            .ForMember(d => d.ClienteId,      o => o.MapFrom(s => s.ClienteId))
            .ForMember(d => d.OrdenServicioId,o => o.MapFrom(s => s.OrdenServicioId))
            .ForMember(d => d.Fecha,          o => o.MapFrom(s => s.Fecha ?? DateTime.UtcNow))
             .ForMember(d => d.TipoPagoId,       o => o.MapFrom(s => s.TipoPagoId))
             .ForMember(d => d.Total,          o => o.MapFrom(s => s.Total))
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // DOMAIN -> RESPONSE
            CreateMap<Factura, FacturaResponse>()
                .ForMember(d => d.ClienteId, o => o.MapFrom(s => s.ClienteId))
                .ForMember(d => d.OrdenServicioId, o => o.MapFrom(s => s.OrdenServicioId))
                .ForMember(d => d.Fecha, o => o.MapFrom(s => s.Fecha))
                .ForMember(d => d.FacturaId, o => o.MapFrom(s => s.FacturaId))
                .ForMember(d => d.TipoPagoId, o => o.MapFrom(s => s.TipoPagoId))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.Total));
        }
    }
}