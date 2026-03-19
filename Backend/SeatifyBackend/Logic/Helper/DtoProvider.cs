using AutoMapper;
using Entities.Dtos.Auditorium;
using Entities.Dtos.Venue;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Logic.Helper;

public class DtoProvider
{
    public Mapper Mapper { get; }
    
    public DtoProvider()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<VenueCreateDto, Venue>();
            cfg.CreateMap<VenueUpdateDto, Venue>();
            cfg.CreateMap<Venue, VenueViewDto>()
                .ForMember(dest => dest.Auditoriums, opt => opt.MapFrom(
                    src => src.Auditoriums.Select(a => new AuditoriumViewDto { Id = a.Id, VenueId = a.VenueId })));
        }, new LoggerFactory());
        Mapper = new Mapper(config);
    }
}
