export class LayoutMatrix {
    id: string = ''
    auditoriumId: string = ''
    name: string = ''
    rows: number = 0
    columns: number = 0
    createdAtUtc: Date = new Date()
    updatedAtUtc: Date = new Date()
}

export interface CreateLayoutMatrixDto {
    name: string
    rows: number
    columns: number
}
