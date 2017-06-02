using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarupanoAirlines.Flight
{
    public class ReservationList
    {
        List<ReservationListItem> _reservations = new List<ReservationListItem>();
        public long LastEventId { get; set; }
        public void On(FlightReservationCreated created)
        {
            _reservations.Add(new ReservationListItem { Localizer = created.Localizer, FlightId = created.Localizer });
        }
        public void On(FlightReservationCancelled cancelled)
        {
            var resv = _reservations.Single(c => c.Localizer == cancelled.Localizer);
            _reservations.Remove(resv);
        }

        public IEnumerable<ReservationListItem> Query(SearchReservationsByFlight query)
        {
            return _reservations.Skip(query.Page * query.PageSize).Take(query.PageSize).Where(c => c.FlightId == query.FlightId);
        }
    }

    public class SearchReservationsByFlight
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string FlightId { get; set; }
    }

    public class ReservationListItem
    {
        public string Localizer { get; set; }
        public string FlightId { get; set; }
    }

    public class FlightReservation
    {
        bool _cancelled;
        public string Localizer { get; private set; }
        public FlightReservationCreated Create(CreateFlightReservation cmd)
        {
            var evt = new FlightReservationCreated
            {
                Localizer = Guid.NewGuid().ToString().Substring(5),
                FlightId = cmd.FlightId,
                PassengerId = cmd.PassengerId
            };
            Apply(evt);
            return evt;
        }
        public FlightReservationCancelled Cancel(CancelFlightReservation cmd)
        {
            if (_cancelled) throw new InvalidOperationException("Reservation already cancelled");
            var evt = new FlightReservationCancelled
            {
                Localizer = cmd.Localizer
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
            Localizer = created.Localizer;
        }
    }

    public class CreateFlightReservation
    {
        public string PassengerId { get; set; }
        public string Localizer { get; set; }
        public string FlightId { get; set; }
        public CreateFlightReservation(string passengerId, string localizer, string flightId)
        {
            PassengerId = passengerId;
            Localizer = localizer;
            FlightId = flightId;

        }
    }
    public class FlightReservationCreated
    {
        public string Localizer { get; set; }
        public string PassengerId { get; set; }
        public string FlightId { get; set; }
    }
    public class CancelFlightReservation
    {
        public string Localizer { get; set; }
    }
    public class FlightReservationCancelled
    {
        public string Localizer { get; set; }
    }
}
