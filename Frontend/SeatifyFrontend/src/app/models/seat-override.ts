export type OverrideSource = 'auditorium' | 'event' | 'occurrence';

export interface EffectiveSeat {
  seatId: string;
  matrixId: string;
  row: number;
  column: number;
  seatLabel: string | null;

  // Effektív értékek
  sectorId: string | null;
  seatType: string;
  priceOverride: number | null;
  finalPrice: number;

  // Honnan jön az érték
  sectorSource: OverrideSource;
  seatTypeSource: OverrideSource;
  priceSource: OverrideSource;
}

export interface EffectiveSeatMap {
  matrixId: string;
  matrixName: string;
  rows: number;
  columns: number;
  context: 'auditorium' | 'event' | 'occurrence';
  eventId: string | null;
  occurrenceId: string | null;
  seats: EffectiveSeat[];
}

export interface BulkSeatOverrideDto {
  seatIds: string[];
  sectorId?: string;
  clearSector?: boolean;
  seatType?: string;
  priceOverride?: number;
  clearPriceOverride?: boolean;
}

export interface BulkSeatOverrideResponseDto {
  upsertedCount: number;
  affectedSeatIds: string[];
}
