using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entities.Dtos.CheckIn;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly AppDbContext _context;

        public CheckInService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CheckInResult> ValidateTicketAsync(string payload)
        {
            var ticketId = payload; // Assuming payload is the ReservationSeat.Id for now
            var result = new CheckInResult { TicketId = ticketId };

            var reservationSeat = await _context.ReservationSeats
                .Include(rs => rs.Reservation)
                .ThenInclude(r => r.EventOccurrence)
                .ThenInclude(e => e.Event)
                .Include(rs => rs.Seat)
                .ThenInclude(s => s.Sector)
                .FirstOrDefaultAsync(rs => rs.Id == ticketId);

            if (reservationSeat == null)
            {
                result.Status = TicketStatus.Invalid;
                result.StatusMessage = "Ticket not found.";
                return result;
            }

            result.Reservation = new ReservationInfo
            {
                CustomerName = reservationSeat.Reservation.CustomerName,
                CustomerEmail = reservationSeat.Reservation.CustomerEmail
            };

            result.Seat = new SeatInfo
            {
                Section = reservationSeat.Seat.Sector?.Name ?? "General",
                Row = reservationSeat.Seat.Row,
                Number = reservationSeat.Seat.Column
            };

            var occurrence = reservationSeat.Reservation.EventOccurrence;
            if (occurrence != null)
            {
                result.Event = new EventInfo
                {
                    Title = occurrence.Event?.Name ?? "Unknown Event",
                    StartTime = occurrence.StartsAtUtc
                };
            }

            if (reservationSeat.IsCheckedIn)
            {
                result.Status = TicketStatus.AlreadyUsed;
                result.StatusMessage = $"Ticket was already checked in at {reservationSeat.CheckInTimeUtc}.";
            }
            else
            {
                result.Status = TicketStatus.Valid;
                result.StatusMessage = "Ticket is valid for check-in.";
            }

            return result;
        }

        public async Task<CheckInResult> ConfirmCheckInAsync(string ticketId)
        {
            var result = await ValidateTicketAsync(ticketId);
            
            if (result.Status == TicketStatus.Valid)
            {
                var reservationSeat = await _context.ReservationSeats.FindAsync(ticketId);
                if (reservationSeat != null)
                {
                    reservationSeat.IsCheckedIn = true;
                    reservationSeat.CheckInTimeUtc = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    result.StatusMessage = "Check-in successful.";
                }
            }

            return result;
        }
    }
}
