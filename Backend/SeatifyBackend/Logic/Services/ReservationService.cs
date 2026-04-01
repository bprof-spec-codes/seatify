using Data;
using Entities.Dtos.Reservation;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class ReservationService: IReservationService
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateReservation(string eventOccurrenceId, ReservationCreateDto dto)
        {
            var reservation = new Reservation
            {
                EventOccurrenceId = eventOccurrenceId,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                Status = "Confirmed",
                CreatedAtUtc = DateTime.UtcNow,
                ReservationSeats = new List<ReservationSeat>()
            };

            foreach (var seat in dto.Seats)
            {
                reservation.ReservationSeats.Add(new ReservationSeat
                {
                    SeatId = seat.SeatId,
                    FinalPrice = 5000 // mock data for testing
                });
            }

            _context.Reservations.Add(reservation);
            return _context.SaveChanges() > 0;
        }

        public ReservationViewDto? GetById(string id)
        {
            var res = _context.Reservations
                .Include(r => r.ReservationSeats)
                .FirstOrDefault(r => r.Id == id);

            if (res == null) return null;

            return new ReservationViewDto
            {
                Id = res.Id,
                CustomerName = res.CustomerName,
                CustomerEmail = res.CustomerEmail,
                Status = res.Status,
                CreatedAtUtc = res.CreatedAtUtc,
                ReservedSeats = res.ReservationSeats.Select(rs => new ReservationSeatViewDto
                {
                    SeatId = rs.SeatId,
                    FinalPrice = rs.FinalPrice
                }).ToList()
            };
        }

        public List<ReservationViewDto> GetByOccurrenceId(string eventOccurrenceId)
        {
            return _context.Reservations
                .Include(r => r.ReservationSeats)
                .Where(r => r.EventOccurrenceId == eventOccurrenceId)
                .Select(res => new ReservationViewDto
                {
                    Id = res.Id,
                    CustomerName = res.CustomerName,
                    CustomerEmail = res.CustomerEmail,
                    Status = res.Status,
                    CreatedAtUtc = res.CreatedAtUtc,
                    ReservedSeats = res.ReservationSeats.Select(rs => new ReservationSeatViewDto
                    {
                        SeatId = rs.SeatId,
                        FinalPrice = rs.FinalPrice
                    }).ToList()
                }).ToList();
        }

        public bool UpdateReservation(string id, ReservationUpdateDto dto)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null) return false;

            res.CustomerName = dto.CustomerName;
            res.CustomerEmail = dto.CustomerEmail;
            res.CustomerPhone = dto.CustomerPhone;
            res.Status = dto.Status;

            return _context.SaveChanges() > 0;
        }

        public bool DeleteReservation(string id)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null) return false;

            _context.Reservations.Remove(res);
            return _context.SaveChanges() > 0;
        }
    }
}
