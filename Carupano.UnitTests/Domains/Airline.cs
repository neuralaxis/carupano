using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.UnitTests.Domains.Airline
{
    public class FlightReservation
    {
        bool _cancelled;
        public FlightReservationCreated Create(CreateFlightReservation cmd)
        {
            var evt = new FlightReservationCreated
            {
                Id = Guid.NewGuid().ToString(),
                PassengerId = cmd.PassengerId,
                Localizer = Guid.NewGuid().ToString().Substring(5)
            };
            Apply(evt);
            return evt;
        }
        public FlightReservationCancelled Cancel(CancelFlightReservation cmd)
        {
            if (_cancelled) throw new InvalidOperationException("Reservation already cancelled");
            var evt = new FlightReservationCancelled
            {
                Id = cmd.Id
            };
            Apply(evt);
            return evt;
        }
        void Apply(FlightReservationCancelled evt)
        {
            _cancelled = true;
        }
        void Apply(FlightReservationCreated created)
        {
            
        }
    }

    public class CreateFlightReservation
    {
        public string PassengerId { get; set; }
        public CreateFlightReservation(string passengerId)
        {
            PassengerId = passengerId;
        }
    }
    public class FlightReservationCreated
    {
        public string Id { get; set; }
        public string PassengerId { get; set; }
        public string Localizer { get; set; }
    }
    public class CancelFlightReservation
    {
        public string Id { get; set; }
    }
    public class FlightReservationCancelled
    {
        public string Id { get; set; }
    }
}
