using System;

namespace CarupanoAir.Booking.Commands
{
    public class CreateReservation
    {
        public string PassengerId { get; }
        public string RouteId { get; }
        public CreateReservation(string passengerId, string routeId)
        {
            PassengerId = passengerId;
            RouteId = routeId;
        }
    }
}
