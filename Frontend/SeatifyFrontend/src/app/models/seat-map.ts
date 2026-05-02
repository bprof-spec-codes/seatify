import { Seat } from "./seat"

export class SeatMap {
    id: string = ''
    rows: number = 0
    columns: number = 0
    currency: string = 'EUR'
    seats: Seat[] = []
}
