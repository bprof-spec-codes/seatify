# Booking Flow Architecture

## Overview

This document summarizes the booking flow architecture for the ticket
booking platform.

The scope of this document is limited to the **booking process** and the
architectural elements that are directly required to support it.

According to the current product direction:

-   the customer selects seats on the public booking page
-   the customer proceeds to a checkout page
-   the system finalizes the reservation
-   tickets are generated with QR data
-   confirmation is sent by email
-   organizers can later review reservations on the admin side

In the current MVP, **there is no online payment**.

The goal of the flow is:

-   seat selection
-   temporary seat holding
-   customer data submission
-   reservation finalization
-   ticket generation
-   email confirmation

------------------------------------------------------------------------

# Core Principle

The bookable unit of the system is **EventOccurrence**, not Event.

This means:

-   `Event` is the content entity
-   `EventOccurrence` is the concrete scheduled instance
-   booking, seat availability, holds, reservations, and tickets must
    all be tied to `EventOccurrence`

Seat availability must always be evaluated in the context of:

Seat + EventOccurrence

------------------------------------------------------------------------

# Main Concepts

## BookingSession

Represents the customer's active booking flow for a specific
`EventOccurrence`.

Responsibilities:

-   grouping the customer's temporary booking work
-   tracking the phase of the booking process
-   storing expiration time
-   linking seat holds belonging to the same session

------------------------------------------------------------------------

## SeatHold

A temporary seat reservation during seat selection.

Responsibilities:

-   preventing other sessions from selecting the same seat
-   linking the seat to the booking session
-   automatically expiring if the user leaves or times out
-   transitioning into a finalized reservation

------------------------------------------------------------------------

## Reservation

Represents a finalized booking created from a valid `BookingSession`.

Responsibilities:

-   storing customer information
-   grouping all seats reserved in the booking
-   serving as the source entity for ticket generation

------------------------------------------------------------------------

## ReservationSeat

Represents the association between a `Reservation` and a `Seat`.

This entity exists because:

-   one reservation can contain multiple seats
-   seat specific booking information may need to be stored
-   it provides a normalized relational structure

Responsibilities:

-   linking seats to a reservation
-   storing seat specific booking information
-   acting as the source for ticket creation

Example JSON:

``` json
{
  "id": "rs_001",
  "reservationId": "res_001",
  "seatId": "seat_A_12",
  "finalPrice": 6500
}
```

------------------------------------------------------------------------

## Ticket

Represents the actual entry ticket generated for a seat.

Responsibilities:

-   providing a unique ticket identifier
-   storing the QR payload used for validation
-   linking the seat entry to the reservation

Example JSON:

``` json
{
  "id": "tkt_001",
  "reservationSeatId": "rs_001",
  "eventOccurrenceId": "occ_001",
  "seatId": "seat_A_12",
  "ticketCode": "TGX9-4M2Q-8L1P",
  "qrPayload": "ticket:tkt_001:TGX9-4M2Q-8L1P",
  "status": "Valid"
}
```

------------------------------------------------------------------------

# Seat States

Seats have three domain states.

-   Available
-   Held
-   Booked

Available -- seat can be selected.

Held -- seat is temporarily reserved by a booking session.

Booked -- seat belongs to a finalized reservation.

------------------------------------------------------------------------

# Booking Session Phases

Booking sessions move through the following phases:

Selection\
Checkout\
Completed\
Expired\
Cancelled

------------------------------------------------------------------------

# Booking Flow

## 1 Public Event Page

User opens the public event page and selects an occurrence.

System loads:

-   event data
-   occurrence data
-   seat layout
-   seat availability

------------------------------------------------------------------------

## 2 First Seat Selection

When a seat is clicked:

-   booking session is created if it does not exist
-   seat hold record is created
-   seat becomes Held

------------------------------------------------------------------------

## 3 Additional Seat Selection

Each seat selected:

-   belongs to the same booking session
-   gets its own SeatHold record

------------------------------------------------------------------------

## 4 Seat Removal

When the user deselects a seat:

-   SeatHold is released
-   seat becomes Available again

------------------------------------------------------------------------

## 5 Checkout

User proceeds to checkout.

System:

-   changes session phase to Checkout
-   keeps seat holds active

------------------------------------------------------------------------

## 6 Reservation Finalization

Backend performs validation:

-   session still active
-   holds still valid

Then:

-   Reservation is created
-   ReservationSeat entries are created
-   Tickets are generated
-   seats become Booked

------------------------------------------------------------------------

# Core Entities

## Event

``` json
{
  "id": "evt_001",
  "slug": "spring-concert-2026",
  "name": "Spring Concert",
  "status": "Published"
}
```

------------------------------------------------------------------------

## EventOccurrence

``` json
{
  "id": "occ_001",
  "eventId": "evt_001",
  "startsAtUtc": "2026-04-20T18:00:00Z",
  "endsAtUtc": "2026-04-20T20:00:00Z"
}
```

------------------------------------------------------------------------

## Seat

``` json
{
  "id": "seat_A_12",
  "rowLabel": "A",
  "seatLabel": "12",
  "basePrice": 6500
}
```

------------------------------------------------------------------------

## BookingSession

``` json
{
  "id": "bs_001",
  "eventOccurrenceId": "occ_001",
  "phase": "Selection",
  "expiresAtUtc": "2026-03-12T18:10:00Z"
}
```

------------------------------------------------------------------------

## SeatHold

``` json
{
  "id": "hold_001",
  "bookingSessionId": "bs_001",
  "seatId": "seat_A_12",
  "eventOccurrenceId": "occ_001"
}
```

------------------------------------------------------------------------

## Reservation

``` json
{
  "id": "res_001",
  "bookingSessionId": "bs_001",
  "eventOccurrenceId": "occ_001",
  "customerEmail": "test@example.com"
}
```

------------------------------------------------------------------------

# API Summary

## Get Public Event

GET /api/public/events/{slug}

------------------------------------------------------------------------

## Get Occurrence Seat Map

GET /api/public/occurrences/{eventOccurrenceId}/seat-map

------------------------------------------------------------------------

## Create Booking Session

POST /api/public/booking-sessions

------------------------------------------------------------------------

## Hold Seat

POST /api/public/booking-sessions/{bookingSessionId}/holds

------------------------------------------------------------------------

## Release Seat

DELETE /api/public/booking-sessions/{bookingSessionId}/holds/{seatId}

------------------------------------------------------------------------

## Move Session to Checkout

POST /api/public/booking-sessions/{bookingSessionId}/checkout

------------------------------------------------------------------------

## Create Reservation

POST /api/public/reservations

------------------------------------------------------------------------

## Get Reservation

GET /api/public/reservations/{reservationId}

------------------------------------------------------------------------

# Concurrency Rules

Two customers must never reserve the same seat simultaneously.

Key rules:

-   backend is source of truth
-   hold creation must be atomic
-   session validity must be checked during reservation
-   expired sessions release holds

------------------------------------------------------------------------

# Domain Diagram

```mermadid
classDiagram
direction LR

class Event {
  +Guid Id
  +Guid OrganizerId
  +string Slug
  +string Name
  +string Description
  +string Status
  +DateTime CreatedAtUtc
  +DateTime UpdatedAtUtc
}

class EventOccurrence {
  +Guid Id
  +Guid EventId
  +Guid VenueId
  +Guid AuditoriumId
  +DateTime StartsAtUtc
  +DateTime EndsAtUtc
  +string Status
  +DateTime BookingOpenAtUtc
  +DateTime BookingCloseAtUtc
  +DateTime CreatedAtUtc
  +DateTime UpdatedAtUtc
}

class BookingSession {
  +Guid Id
  +Guid EventOccurrenceId
  +string Phase
  +string Status
  +DateTime CreatedAtUtc
  +DateTime ExpiresAtUtc
}

class Seat {
  +Guid Id
  +Guid LayoutMatrixId
  +Guid SectorId
  +string RowLabel
  +string SeatLabel
  +int X
  +int Y
  +decimal BasePrice
  +string SeatType
}

class SeatHold {
  +Guid Id
  +Guid BookingSessionId
  +Guid EventOccurrenceId
  +Guid SeatId
  +string Phase
  +string Status
  +DateTime CreatedAtUtc
}

class Reservation {
  +Guid Id
  +Guid BookingSessionId
  +Guid EventOccurrenceId
  +string CustomerName
  +string CustomerEmail
  +string CustomerPhone
  +string Status
  +DateTime CreatedAtUtc
}

class ReservationSeat {
  +Guid Id
  +Guid ReservationId
  +Guid SeatId
  +decimal FinalPrice
}

class Ticket {
  +Guid Id
  +Guid ReservationId
  +Guid EventOccurrenceId
  +Guid SeatId
  +string TicketCode
  +string QrPayload
  +string Status
  +DateTime IssuedAtUtc
}

Event "1" --> "0..*" EventOccurrence : has
EventOccurrence "1" --> "0..*" BookingSession : starts
EventOccurrence "1" --> "0..*" SeatHold : contains
BookingSession "1" --> "0..*" SeatHold : owns
Seat "1" --> "0..*" SeatHold : temporarily held in

BookingSession "1" --> "0..1" Reservation : finalizes into
EventOccurrence "1" --> "0..*" Reservation : results in
Reservation "1" --> "1..*" ReservationSeat : contains
Seat "1" --> "0..*" ReservationSeat : reserved as

EventOccurrence "1" --> "0..*" Ticket : issues for
Reservation "1" --> "1..*" Ticket : generates
Seat "1" --> "0..*" Ticket : ticketed as
```

# Summary

The booking subsystem centers around `BookingSession`, which manages
seat holds and transitions into a finalized `Reservation`.

Seats selected by the user become `SeatHold` records. When the booking
is finalized, `ReservationSeat` entities are created to associate seats
with the reservation, and `Ticket` objects are generated for entry
validation.

This architecture supports:

-   multiple occurrences per event
-   reusable seat layouts
-   temporary seat locking
-   safe reservation finalization
-   reliable ticket generation
