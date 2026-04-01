import { SeatType } from "./seat"
import { OverrideSource } from "./seat-override"

export interface MatrixCellVm {
  key: string
  row: number
  column: number
  seatId: string | null
  seatLabel: string | null
  seatType: SeatType
  sectorId: string | null
  priceOverride: number | null
  /** Honnan jön a szektor értéke (csak override módban értelmes) */
  sectorSource?: OverrideSource
  /** Honnan jön az ár értéke */
  priceSource?: OverrideSource
  /** Honnan jön a kategória értéke */
  seatTypeSource?: OverrideSource
}
