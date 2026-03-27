import { Auditorium } from "./auditorium";

export class Venue {
    id: string | undefined = '';
    name: string = '';
    city: string = '';
    postalCode: string = '';
    addressLine: string = '';
    auditoriums: Auditorium[] = [];
    organizerId: string | undefined = '';
}
