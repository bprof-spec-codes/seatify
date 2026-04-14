using Data;
using Entities.Dtos.BookingSession;
using Entities.Models;
using Logic.Helper;

namespace Logic.Services;

public class BookingService
{
    private readonly AppDbContext _ctx;
    private readonly DtoProvider dtoProvider;

    public BookingService(AppDbContext ctx, DtoProvider dtoProvider)
    {
        _ctx = ctx;
        this.dtoProvider = dtoProvider;
    }
    
    public async Task<BookingSession> CreateBookingSessionAsync(BookingSessionCreateDto createDto)
    {
        var newBookingSession = dtoProvider.Mapper.Map<BookingSession>(createDto);

        await _ctx.BookingSessions.AddAsync(newBookingSession);
        await _ctx.SaveChangesAsync();

        return newBookingSession;
    }
}
