import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription, forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { PublicBookingStateService, SelectedSeat } from '../../services/public-booking-state.service';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { SeatOverrideService } from '../../services/seat-override.service';
import { ReservationService } from '../../services/reservation.service';
import { SectorService } from '../../services/sector.service';
import { EventService } from '../../services/event.service';
import { BookingSessionService } from '../../services/booking-session.service';
import { EffectiveSeatMap, EffectiveSeat } from '../../models/seat-override';
import { Sector } from '../../models/sector';
import { EventOccurrence } from '../../models/event-occurrence';
import { BookingSession } from '../../models/booking.model';

interface GridCell {
  key: string;
  row: number;
  column: number;
  seatId: string | null;
  seatLabel: string | null;
  rowLabel: string | null;
  seatType: string;
  price: number;
  isBooked: boolean;
  isSelected: boolean;
  isHeld: boolean;
  sectorId: string | null;
}

@Component({
  selector: 'app-public-booking-map',
  standalone: false,
  templateUrl: './public-booking-map.component.html',
  styleUrls: ['./public-booking-map.component.sass']
})
export class PublicBookingMapComponent implements OnInit, OnDestroy {
  isLoading = true;
  errorMessage = '';
  noticeMessage = '';
  isBlockingError = false;

  gridCells: GridCell[] = [];
  gridRows = 0;
  gridColumns = 0;
  
  selectedSeats: SelectedSeat[] = [];
  totalPrice = 0;
  currency = 'EUR';

  sectors: Sector[] = [];
  priceCategories: { name: string, price: number, color: string }[] = [];

  countdownText = '';
  countdownSeconds = 0;
  showCountdown = false;

  private occurrenceId: string | null = null;
  private bookingSessionId: string | null = null;
  private isHoldRequestInFlight = false;
  private countdownTimerId: number | null = null;

  private sub = new Subscription();

  constructor(
    private stateService: PublicBookingStateService,
    private matrixService: LayoutMatrixService,
    private seatOverrideService: SeatOverrideService,
    private reservationService: ReservationService,
    private sectorService: SectorService,
    private eventService: EventService,
    private bookingSessionService: BookingSessionService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isLoading = true;

    // Wait for occurrence from state OR fetch it from the URL
    this.occurrenceId = this.route.parent?.snapshot.paramMap.get('occurrenceId') ?? null;
    if (!this.occurrenceId) {
      this.errorMessage = 'Invalid occurrence ID.';
      this.isBlockingError = true;
      this.isLoading = false;
      return;
    }

    this.bookingSessionId = this.stateService.getBookingSessionId();

    const stateOcc = this.stateService.getEventOccurrence();
    if (stateOcc && stateOcc.id === this.occurrenceId) {
      this.currency = stateOcc.effectiveCurrency;
      this.loadData(stateOcc);
    } else {
      // Fetch specifically if state is not ready (normal for direct links)
      this.sub.add(
        this.eventService.getOccurrenceById(this.occurrenceId).subscribe({
          next: (occ) => {
            this.stateService.setEventOccurrence(occ);
            this.currency = occ.effectiveCurrency;
            this.loadData(occ);
          },
          error: (err) => {
            this.errorMessage = 'Event occurrence not found.';
            this.isBlockingError = true;
            this.isLoading = false;
          }
        })
      );
    }
  }

  loadData(occ: EventOccurrence): void {
    this.isLoading = true;
    
    this.sub.add(
      this.matrixService.getLayoutMatrixByAuditoriumId(occ.auditoriumId).subscribe({
        next: (matrices) => {
          if (!matrices || matrices.length === 0) {
            this.errorMessage = 'No seating layout found for this auditorium.';
            this.isBlockingError = true;
            this.isLoading = false;
            return;
          }

          const matrixId = matrices[0].id;

          this.sub.add(
            forkJoin({
              seatMap: this.seatOverrideService.getEffectiveSeatMapForOccurrence(occ.id, matrixId),
              reservations: this.reservationService.getReservationsForOccurrence(occ.id).pipe(catchError(() => of([]))),
              sectors: this.sectorService.getSectorsByAuditoriumId(occ.auditoriumId).pipe(catchError(() => of([])))
            }).subscribe({
              next: ({ seatMap, reservations, sectors }) => {
                this.sectors = sectors;
                this.currency = seatMap.currency || 'EUR';
                this.buildGrid(seatMap, reservations);
                this.calculatePriceCategories();
                this.isLoading = false;
              },
              error: (err) => {
                this.errorMessage = 'Failed to load seating map.';
                this.isBlockingError = true;
                this.isLoading = false;
                console.error(err);
              }
            })
          );
        },
        error: (err) => {
          this.errorMessage = 'Failed to load layout data.';
          this.isBlockingError = true;
          this.isLoading = false;
          console.error(err);
        }
      })
    );
  }

  buildGrid(seatMap: EffectiveSeatMap, reservations: any[]): void {
    this.gridRows = seatMap.rows;
    this.gridColumns = seatMap.columns;
    
    const bookedSeatIds = new Set<string>();
    reservations.forEach(res => {
      if (res.status === 'Confirmed') {
        res.reservedSeats.forEach((rs: any) => bookedSeatIds.add(rs.seatId));
      }
    });

    const cells: GridCell[] = [];
    const seatLookup = new Map<string, EffectiveSeat>();
    seatMap.seats.forEach(s => seatLookup.set(`${s.row}-${s.column}`, s));

    for (let r = 1; r <= seatMap.rows; r++) {
      for (let c = 1; c <= seatMap.columns; c++) {
        const key = `${r}-${c}`;
        const seat = seatLookup.get(key);
        
        cells.push({
          key,
          row: r,
          column: c,
          seatId: seat?.seatId || null,
          seatLabel: seat?.seatLabel || null,
          rowLabel: seat?.rowLabel || null,
          seatType: seat?.seatType || 'Aisle',
          price: seat?.finalPrice || 0,
          isBooked: seat?.seatId ? bookedSeatIds.has(seat.seatId) : false,
          isSelected: false,
          isHeld: false,
          sectorId: seat?.sectorId || null
        });
      }
    }
    this.gridCells = cells;
    if (this.bookingSessionId) {
      this.refreshFromSession(this.bookingSessionId);
    }
  }

  calculatePriceCategories(): void {
    const categoriesMap = new Map<number, { name: string, color: string }>();
    
    this.sectors.forEach(s => {
      categoriesMap.set(s.basePrice, { name: s.name, color: s.color });
    });

    this.gridCells.forEach(c => {
      if (c.seatId && c.seatType === 'Seat' && !categoriesMap.has(c.price)) {
        categoriesMap.set(c.price, { name: 'Special Price', color: '#636e72' });
      }
    });

    this.priceCategories = Array.from(categoriesMap.entries()).map(([price, info]) => ({
      price,
      name: info.name,
      color: info.color
    })).sort((a, b) => b.price - a.price);
  }

  getSectorColor(sectorId: string | null): string {
    if (!sectorId) return '#ffffff';
    const sector = this.sectors.find(s => s.id === sectorId);
    return sector ? sector.color : '#ffffff';
  }

  toggleSeat(cell: GridCell): void {
    if (cell.isBooked || cell.isHeld || !cell.seatId || cell.seatType !== 'Seat') return;
    if (this.isHoldRequestInFlight || !this.occurrenceId) return;

    this.errorMessage = '';
    this.noticeMessage = '';
    this.isHoldRequestInFlight = true;

    const seatId = cell.seatId;
    const request$ = this.ensureBookingSession().pipe(
      switchMap(sessionId =>
        cell.isSelected
          ? this.bookingSessionService.releaseSeat(sessionId, seatId)
          : this.bookingSessionService.holdSeat(sessionId, seatId)
      )
    );

    this.sub.add(
      request$.subscribe({
        next: (session) => {
          this.applySessionHolds(session);
          this.isHoldRequestInFlight = false;
        },
        error: (err) => {
          this.isHoldRequestInFlight = false;
          const message = err.error?.message || 'Failed to update seat selection.';
          if (this.isExpiredMessage(message)) {
            this.handleExpiredSession();
          } else if (this.isHeldMessage(message)) {
            cell.isHeld = true;
            cell.isSelected = false;
            this.updateSelection();
            this.noticeMessage = message;
          } else {
            this.noticeMessage = message;
          }
        }
      })
    );
  }

  updateSelection(): void {
    this.selectedSeats = this.gridCells
      .filter(c => c.isSelected && c.seatId)
      .map(c => ({
        seatId: c.seatId!,
        seatLabel: c.seatLabel || c.seatId!,
        rowLabel: c.rowLabel || c.seatId!,
        price: c.price
      }));
    
    this.totalPrice = this.selectedSeats.reduce((sum, s) => sum + s.price, 0);
    this.stateService.setSelectedSeats(this.selectedSeats);
  }

  proceedToCheckout(): void {
    if (this.selectedSeats.length === 0) return;
    if (!this.occurrenceId) return;

    this.errorMessage = '';
    this.noticeMessage = '';

    this.sub.add(
      this.ensureBookingSession()
        .pipe(switchMap(sessionId => this.bookingSessionService.checkout(sessionId).pipe(map(() => sessionId))))
        .subscribe({
          next: (sessionId) => {
            this.router.navigate(['./checkout'], {
              relativeTo: this.route,
              queryParams: { sessionId }
            });
          },
          error: (err) => {
            const message = err.error?.message || 'Failed to start checkout. Please try again.';
            if (this.isExpiredMessage(message)) {
              this.handleExpiredSession();
            } else {
              this.noticeMessage = message;
            }
          }
        })
    );
  }

  private isExpiredMessage(message: string): boolean {
    return message.toLowerCase().includes('expired');
  }

  private isHeldMessage(message: string): boolean {
    return message.toLowerCase().includes('held by another session');
  }

  private ensureBookingSession() {
    if (!this.occurrenceId) {
      return of(null).pipe(
        map(() => {
          throw new Error('Missing occurrence ID.');
        })
      );
    }

    if (this.bookingSessionId) {
      return of(this.bookingSessionId);
    }

    return this.bookingSessionService.createBookingSession(this.occurrenceId).pipe(
      map(session => {
        this.bookingSessionId = session.id;
        this.stateService.setBookingSessionId(session.id);
        return session.id;
      })
    );
  }

  private refreshFromSession(sessionId: string): void {
    this.sub.add(
      this.bookingSessionService.getActiveSession(sessionId).subscribe({
        next: (session) => this.applySessionHolds(session),
        error: () => {
          this.stateService.setBookingSessionId(null);
          this.bookingSessionId = null;
        }
      })
    );
  }

  private applySessionHolds(session: BookingSession): void {
    if (session.status !== 'Active' || session.phase === 'Expired' || session.phase === 'Cancelled') {
      this.handleExpiredSession();
      return;
    }

    this.bookingSessionId = session.id;
    this.stateService.setBookingSessionId(session.id);

    const heldSeatIds = new Set(session.holds.map(h => h.seatId));
    this.gridCells.forEach(cell => {
      if (!cell.seatId) return;
      cell.isHeld = false;
      cell.isSelected = heldSeatIds.has(cell.seatId);
    });

    this.selectedSeats = session.holds.map(hold => ({
      seatId: hold.seatId,
      seatLabel: hold.seatLabel || hold.seatId,
      rowLabel: hold.rowLabel || hold.seatId,
      price: hold.basePrice
    }));

    this.totalPrice = this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
    this.stateService.setSelectedSeats(this.selectedSeats);

    this.updateCountdown(session.expiresAtUtc);
  }

  private handleExpiredSession(): void {
    this.noticeMessage = 'Your booking session has expired. Please select your seats again.';
    this.bookingSessionId = null;
    this.stateService.setBookingSessionId(null);

    this.clearCountdown();

    this.gridCells.forEach(cell => {
      if (!cell.seatId) return;
      cell.isSelected = false;
      cell.isHeld = false;
    });

    this.selectedSeats = [];
    this.totalPrice = 0;
    this.stateService.setSelectedSeats([]);
  }

  private updateCountdown(expiresAtUtc: string): void {
    const expiresAt = new Date(expiresAtUtc).getTime();
    if (Number.isNaN(expiresAt)) {
      this.clearCountdown();
      return;
    }

    this.showCountdown = true;

    const refresh = () => {
      const now = Date.now();
      const remainingMs = Math.max(0, expiresAt - now);
      this.countdownSeconds = Math.ceil(remainingMs / 1000);

      if (remainingMs <= 0) {
        this.handleExpiredSession();
        return;
      }

      this.countdownText = this.formatCountdown(this.countdownSeconds);
    };

    if (this.countdownTimerId !== null) {
      window.clearInterval(this.countdownTimerId);
    }

    refresh();
    this.countdownTimerId = window.setInterval(refresh, 1000);
  }

  private formatCountdown(totalSeconds: number): string {
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  private clearCountdown(): void {
    this.showCountdown = false;
    this.countdownText = '';
    this.countdownSeconds = 0;
    if (this.countdownTimerId !== null) {
      window.clearInterval(this.countdownTimerId);
      this.countdownTimerId = null;
    }
  }

  ngOnDestroy(): void {
    this.clearCountdown();
    this.sub.unsubscribe();
  }
}
