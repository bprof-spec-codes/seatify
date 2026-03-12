
# Seat Layout Architecture

## Overview

This document describes the architecture and creation workflow of the **Seat Layout system** used in the ticket booking platform.

The seat layout defines the physical seating structure of a venue’s auditorium and is later reused by the booking system when creating event occurrences.

The seat layout system is responsible for:

- defining venue seating structure
- generating seat records
- grouping seats into sectors
- configuring seat properties
- providing layout preview data for the frontend
- serving as the base template for booking during event occurrences

Seat layouts are **templates** describing the static structure of an auditorium.  
Bookings are always evaluated relative to a specific **EventOccurrence**.

---

# Domain Model

The architecture separates **static layout configuration** from **dynamic event scheduling and booking**.

### Static layout entities

- Organizer
- Venue
- Auditorium
- LayoutMatrix
- Sector
- Seat

### Dynamic scheduling entities

- Event
- EventOccurrence

---

# Entity Relationships

## Ownership structure

```
Organizer
├── Events
│    └── EventOccurrences
│
└── Venues
     └── Auditoriums
          ├── LayoutMatrices
          │     └── Seats
          └── Sectors
```

## Booking context

```
EventOccurrence
└── Seat
```

Seat availability is always evaluated in the context of a specific **EventOccurrence**.

---

# Entities

## Venue

Represents a physical location such as a cinema, theatre, or concert hall.

Relationship:

```
Organizer 1 → N Venue
Venue 1 → N Auditorium
```

Example JSON:

```json
{
  "id": "venue-001",
  "organizerId": "org-001",
  "name": "Corvin Cinema",
  "city": "Budapest",
  "postalCode": "1082",
  "addressLine": "Corvin köz 1"
}
```

---

## Auditorium

Represents a specific hall or seating area within a venue.

Examples include cinema halls or theatre stages.

Relationship:

```
Venue 1 → N Auditorium
```

Example JSON:

```json
{
  "id": "aud-001",
  "venueId": "venue-001",
  "name": "Hall 1"
}
```

---

## LayoutMatrix

Defines a rectangular seating grid inside an auditorium.

Each matrix describes:

- number of rows
- number of columns

Example JSON:

```json
{
  "id": "matrix-001",
  "auditoriumId": "aud-001",
  "name": "Main Block",
  "rows": 6,
  "columns": 10
}
```

When a matrix is created, seats are automatically generated.

---

## Sector

Sectors represent seat groupings used for pricing and categorization.

Example JSON:

```json
{
  "id": "sector-001",
  "auditoriumId": "aud-001",
  "name": "VIP",
  "color": "#DC2626",
  "basePrice": 7990
}
```

---

## Seat

Represents a single selectable seat.

Seat properties:

- row
- column
- sector reference
- seat type
- optional price override

Example JSON:

```json
{
  "id": "seat-001",
  "layoutMatrixId": "matrix-001",
  "row": 1,
  "column": 1,
  "seatLabel": "A1",
  "sectorId": "sector-001",
  "seatType": "Seat",
  "priceOverride": null
}
```

---

# Seat Layout Creation Workflow

## Step 1 — Create Auditorium

API:

```
POST /api/venues/{venueId}/auditoriums
```

Request:

```json
{
  "name": "Hall 1"
}
```

Response:

```json
{
  "id": "aud-001",
  "venueId": "venue-001",
  "name": "Hall 1"
}
```

---

## Step 2 — Create Layout Matrix

API:

```
POST /api/auditoriums/{auditoriumId}/matrices
```

Request:

```json
{
  "name": "Main Block",
  "rows": 6,
  "columns": 10
}
```

Response:

```json
{
  "id": "matrix-001",
  "auditoriumId": "aud-001",
  "name": "Main Block",
  "rows": 6,
  "columns": 10
}
```

Seat records are automatically generated after this step.

---

## Step 3 — Retrieve Seats

API:

```
GET /api/matrices/{matrixId}/seats
```

Response:

```json
{
  "matrixId": "matrix-001",
  "rows": 6,
  "columns": 10,
  "seats": [
    {
      "id": "seat-001",
      "row": 1,
      "column": 1,
      "seatType": "Seat"
    },
    {
      "id": "seat-002",
      "row": 1,
      "column": 2,
      "seatType": "Seat"
    }
  ]
}
```

---

## Step 4 — Create Sector

API:

```
POST /api/auditoriums/{auditoriumId}/sectors
```

Request:

```json
{
  "name": "Standard",
  "color": "#2563EB",
  "basePrice": 4990
}
```

Response:

```json
{
  "id": "sector-001",
  "name": "Standard",
  "basePrice": 4990
}
```

---

## Step 5 — Bulk Seat Configuration

API:

```
PATCH /api/seats/bulk
```

Request:

```json
{
  "seatIds": ["seat-001", "seat-002", "seat-003"],
  "sectorId": "sector-001"
}
```

Response:

```json
{
  "updatedSeatCount": 3
}
```

Bulk operations allow assigning:

- sector
- seat type
- price overrides

---

## Step 6 — Layout Preview

API:

```
GET /api/auditoriums/{auditoriumId}/layout-preview
```

Response:

```json
{
  "auditorium": {
    "id": "aud-001",
    "name": "Hall 1"
  },
  "sectors": [
    {
      "id": "sector-001",
      "name": "Standard",
      "color": "#2563EB",
      "basePrice": 4990
    }
  ],
  "matrices": [
    {
      "id": "matrix-001",
      "rows": 6,
      "columns": 10,
      "seats": [
        {
          "id": "seat-001",
          "row": 1,
          "column": 1,
          "sectorId": "sector-001"
        }
      ]
    }
  ]
}
```

The frontend uses this endpoint to render the complete seat layout preview.

---

# Seat Layout as Template

Seat layouts are reusable structures used by event occurrences.

```
EventOccurrence → Auditorium → LayoutMatrix → Seat
```

The booking system references the same seat structure for each occurrence.

---

# Booking Context

Bookings reference both the occurrence and the seat.

```
Booking
├── EventOccurrenceId
└── SeatId
```

This architecture allows the same layout to be reused for multiple occurrences of an event.

---

# ClassDiagram

```mermaid
classDiagram
direction LR

class Organizer {
  +Guid Id
  +string DisplayName
  +string Email
}

class Venue {
  +Guid Id
  +Guid OrganizerId
  +string Name
  +string City
  +string PostalCode
  +string AddressLine
}

class Auditorium {
  +Guid Id
  +Guid VenueId
  +string Name
}

class LayoutMatrix {
  +Guid Id
  +Guid AuditoriumId
  +string Name
  +int Rows
  +int Columns
}

class Sector {
  +Guid Id
  +Guid AuditoriumId
  +string Name
  +string Color
  +decimal BasePrice
}

class Seat {
  +Guid Id
  +Guid LayoutMatrixId
  +Guid SectorId
  +int Row
  +int Column
  +string SeatLabel
  +string SeatType
  +decimal PriceOverride
}

class Event {
  +Guid Id
  +Guid OrganizerId
  +string Name
  +string Description
  +string Slug
  +string Status
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
}

Organizer "1" --> "0..*" Venue : owns
Organizer "1" --> "0..*" Event : owns

Venue "1" --> "0..*" Auditorium : contains
Auditorium "1" --> "0..*" LayoutMatrix : contains
Auditorium "1" --> "0..*" Sector : defines

LayoutMatrix "1" --> "0..*" Seat : contains
Sector "1" --> "0..*" Seat : groups

Event "1" --> "0..*" EventOccurrence : has
EventOccurrence "*" --> "1" Venue : takes place at
EventOccurrence "*" --> "1" Auditorium : uses

# Architectural Summary

Static layout configuration:

- Venue
- Auditorium
- LayoutMatrix
- Sector
- Seat

Dynamic scheduling:

- Event
- EventOccurrence

Key principles:

- seat layouts are reusable templates
- events may have multiple occurrences
- seat availability is evaluated per occurrence

