import { Auditorium } from "./auditorium";

export class Venue {
    id: string = '';
    name: string = '';
    city: string = '';
    postalCode: string = '';
    addressLine: string = '';
    auditoriums: Auditorium[] = [];
}
