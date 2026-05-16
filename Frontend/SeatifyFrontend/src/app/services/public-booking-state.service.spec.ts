import { TestBed } from '@angular/core/testing';
import { PublicBookingStateService, SelectedSeat } from './public-booking-state.service';
import { EventOccurrence } from '../models/event-occurrence';

describe('PublicBookingStateService', () => {
  let service: PublicBookingStateService;

  const sampleSeats: SelectedSeat[] = [
    { seatId: 's1', seatLabel: 'A1', price: 10 },
    { seatId: 's2', seatLabel: 'A2', price: 15 },
  ];

  const sampleEvent: EventOccurrence = {
    id: 'e1',
    eventId: 'ev1',
    venueId: 'v1',
    auditoriumId: 'a1',
    startsAtUtc: '2026-06-01T19:00:00Z',
    endsAtUtc: '2026-06-01T21:00:00Z',
    bookingOpenAtUtc: '2026-05-01T00:00:00Z',
    bookingCloseAtUtc: '2026-06-01T18:00:00Z',
    status: 'active',
    effectiveCurrency: 'USD',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PublicBookingStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('initial selected seats should be empty and event occurrence null', () => {
    expect(service.getSelectedSeats()).toEqual([]);
    expect(service.getEventOccurrence()).toBeNull();
  });

  it('setSelectedSeats should update value and emit via selectedSeats$', (done) => {
    service.selectedSeats$.subscribe(seats => {
      if (seats.length) {
        expect(seats).toEqual(sampleSeats);
        expect(service.getSelectedSeats()).toEqual(sampleSeats);
        done();
      }
    });

    service.setSelectedSeats(sampleSeats);
  });

  it('getTotalPrice should return correct sum', () => {
    service.setSelectedSeats(sampleSeats);
    expect(service.getTotalPrice()).toBe(25);
  });

  it('setEventOccurrence should update value and emit via eventOccurrence$', (done) => {
    service.eventOccurrence$.subscribe(occ => {
      if (occ) {
        expect(occ).toEqual(sampleEvent);
        expect(service.getEventOccurrence()).toEqual(sampleEvent);
        done();
      }
    });

    service.setEventOccurrence(sampleEvent);
  });

  it('clearState should reset selected seats and event occurrence', (done) => {
    service.setSelectedSeats(sampleSeats);
    service.setEventOccurrence(sampleEvent);

    let seatsCleared = false;
    let eventCleared = false;

    service.selectedSeats$.subscribe(seats => {
      if (seats.length === 0) seatsCleared = true;
      maybeDone();
    });

    service.eventOccurrence$.subscribe(occ => {
      if (occ === null) eventCleared = true;
      maybeDone();
    });

    service.clearState();

    function maybeDone() {
      if (seatsCleared && eventCleared) {
        expect(service.getSelectedSeats()).toEqual([]);
        expect(service.getEventOccurrence()).toBeNull();
        done();
      }
    }
  });
});
