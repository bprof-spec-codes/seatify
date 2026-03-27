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
    Seat = 0,
    AccessibleSeat = 1,
    Aisle = 2
}
