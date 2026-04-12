export class Sector {
    id: string = ''
    auditoriumId: string = ''
    name: string = ''
    color: string = '#FFFFFF'
    basePrice: number = 0
    createdAtUtc: Date = new Date()
    updatedAtUtc: Date = new Date()
}

export interface CreateUpdateSectorDto {
    name: string
    color: string
    basePrice: number
}

export interface SectorViewDto {
    id: string
    auditoriumId: string
    name: string
    color: string
    basePrice: number
    createdAtUtc: string
    updatedAtUtc: string
}
