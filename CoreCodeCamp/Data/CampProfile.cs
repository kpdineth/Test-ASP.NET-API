using AutoMapper;
using CoreCodeCamp.Data.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c=>c.Venue,o=>o.MapFrom(m=>m.Location.VenueName));
            CreateMap<CampModel,Camp>()
                .ForPath(c=>c.Location.VenueName,o=>o.MapFrom(m=>m.Venue))
                .ForPath(c=>c.Location.Address1,o=>o.MapFrom(m=>m.LocationAddress1))
                .ForPath(c=>c.Location.Address2,o=>o.MapFrom(m=>m.LocationAddress2))
                .ForPath(c=>c.Location.Address3,o=>o.MapFrom(m=>m.LocationAddress3));

            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t=>t.Camp,o=>o.Ignore())
                .ForMember(t=>t.Speaker,o=>o.Ignore());


            CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
            CreateMap<Location, Location>();
        }
    }
}
