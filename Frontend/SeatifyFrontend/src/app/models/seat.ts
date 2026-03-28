export class Seat {
    id: string = ''
    matrixId: string = ''
    row: number = 0
    column: number = 0
    seatLabel?: string
    sectorId?: string
    priceOverride?: number
    seatType: SeatType = SeatType.Seat
    createdAtUtc: Date = new Date()
    updatedAtUtc: Date = new Date()
}

export enum SeatType {
    Seat = 'Seat',
    AccessibleSeat = 'AccessibleSeat',
    Aisle = 'Aisle'
}

export interface UpdateSeatDto {
    seatLabel: string | null
    sectorId: string | null
    priceOverride: number | null
    seatType: SeatType
}
