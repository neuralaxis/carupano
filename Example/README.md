# Carupano Air
## A non-trivial example

Carupano Air is an airline, and here's their software.

## Domain

### Bounded Contexts
- Route
- Passenger
- Booking

### Aggregates
- Reservation
- Notification
- Flight
- Passenger

### Value Objects
- Route

### Projections
- FlightList
- PassengerList
- ReservationList

### Queries
- GetFlights
- FindPassengerByEmail
- FindReservationsById
- FindReservationsByEmail
- FindReservationsByFlightId
