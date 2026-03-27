import { SeatType } from "./seat"

export interface MatrixCellVm {
  key: string
  row: number
  column: number
  seatId: string | null
  seatLabel: string | null
  seatType: SeatType
  sectorId: string | null
  priceOverride: number | null
}
