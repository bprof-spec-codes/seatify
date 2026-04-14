using AutoMapper;
using Entities.Dtos.Auditorium;
using Entities.Dtos.BookingSession;
using Entities.Dtos.Event;
using Entities.Dtos.LayoutMatrix;
using Entities.Dtos.Seat;
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
                    src => src.Auditoriums.Select(a => new AuditoriumViewDto
                    {
                        Id = a.Id,
                        VenueId = a.VenueId,
                        Name = a.Name,
                        Description = a.Description,
                        CreatedAtUtc = a.CreatedAtUtc,
                        UpdatedAtUtc = a.UpdatedAtUtc
                    })));

            cfg.CreateMap<LayoutMatrixCreateDto, LayoutMatrix>();

            cfg.CreateMap<LayoutMatrixUpdateDto, LayoutMatrix>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AuditoriumId, opt => opt.Ignore())
                .ForMember(dest => dest.Auditorium, opt => opt.Ignore())
                .ForMember(dest => dest.Seats, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

            cfg.CreateMap<LayoutMatrix, LayoutMatrixViewDto>();
            cfg.CreateMap<Seat, SeatViewDto>();

            cfg.CreateMap<LayoutMatrix, LayoutMatrixSeatMapDto>()
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

            cfg.CreateMap<EventCreateDto, Event>();

            cfg.CreateMap<EventUpdateDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
                .ForMember(dest => dest.EventOccurrences, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

            cfg.CreateMap<Event, EventViewDto>();
            
            cfg.CreateMap<BookingSessionCreateDto, BookingSession>();
        }, new LoggerFactory());

        Mapper = new Mapper(config);
    }
}
