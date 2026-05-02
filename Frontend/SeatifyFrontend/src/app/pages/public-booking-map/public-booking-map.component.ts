import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription, forkJoin, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { PublicBookingStateService, SelectedSeat } from '../../services/public-booking-state.service';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { SeatOverrideService } from '../../services/seat-override.service';
import { ReservationService } from '../../services/reservation.service';
import { SectorService } from '../../services/sector.service';
import { EventService } from '../../services/event.service';
import { EffectiveSeatMap, EffectiveSeat } from '../../models/seat-override';
import { Sector } from '../../models/sector';
import { EventOccurrence } from '../../models/event-occurrence';

interface GridCell {
  key: string;
  row: number;
  column: number;
  seatId: string | null;
  seatLabel: string | null;
  seatType: string;
  price: number;
  isBooked: boolean;
  isSelected: boolean;
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

  gridCells: GridCell[] = [];
  gridRows = 0;
  gridColumns = 0;
  
  selectedSeats: SelectedSeat[] = [];
  totalPrice = 0;
  currency = 'EUR';

  sectors: Sector[] = [];
  priceCategories: { name: string, price: number, color: string }[] = [];

  private sub = new Subscription();

  constructor(
    private stateService: PublicBookingStateService,
    private matrixService: LayoutMatrixService,
    private seatOverrideService: SeatOverrideService,
    private reservationService: ReservationService,
    private sectorService: SectorService,
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isLoading = true;

    // Wait for occurrence from state OR fetch it from the URL
    const occurrenceId = this.route.parent?.snapshot.paramMap.get('occurrenceId');
    if (!occurrenceId) {
      this.errorMessage = 'Invalid occurrence ID.';
      this.isLoading = false;
      return;
    }

    const stateOcc = this.stateService.getEventOccurrence();
    if (stateOcc && stateOcc.id === occurrenceId) {
      this.currency = stateOcc.effectiveCurrency;
      this.loadData(stateOcc);
    } else {
      // Fetch specifically if state is not ready (normal for direct links)
      this.sub.add(
        this.eventService.getOccurrenceById(occurrenceId).subscribe({
          next: (occ) => {
            this.stateService.setEventOccurrence(occ);
            this.currency = occ.effectiveCurrency;
            this.loadData(occ);
          },
          error: (err) => {
            this.errorMessage = 'Event occurrence not found.';
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
                this.isLoading = false;
                console.error(err);
              }
            })
          );
        },
        error: (err) => {
          this.errorMessage = 'Failed to load layout data.';
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
          seatType: seat?.seatType || 'Aisle',
          price: seat?.finalPrice || 0,
          isBooked: seat?.seatId ? bookedSeatIds.has(seat.seatId) : false,
          isSelected: false,
          sectorId: seat?.sectorId || null
        });
      }
    }
    this.gridCells = cells;
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
    if (cell.isBooked || !cell.seatId || cell.seatType !== 'Seat') return;

    cell.isSelected = !cell.isSelected;
    this.updateSelection();
  }

  updateSelection(): void {
    this.selectedSeats = this.gridCells
      .filter(c => c.isSelected && c.seatId)
      .map(c => ({
        seatId: c.seatId!,
        seatLabel: c.seatLabel || c.seatId!,
        price: c.price
      }));
    
    this.totalPrice = this.selectedSeats.reduce((sum, s) => sum + s.price, 0);
    this.stateService.setSelectedSeats(this.selectedSeats);
  }

  proceedToCheckout(): void {
    if (this.selectedSeats.length === 0) return;
    this.router.navigate(['./checkout'], { relativeTo: this.route });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
