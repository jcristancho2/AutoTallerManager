using AutoMapper;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Mappings
{
    public class SpareProfile : Profile
    {
        public SpareProfile() 
        {
            CreateMap<Spare, Spare>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}