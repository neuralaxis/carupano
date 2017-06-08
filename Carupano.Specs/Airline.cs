using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarupanoAirlines.Flight
{
    public class ReservationViewRepository
    {
        public List<ReservationView> List { get; } = new List<ReservationView>();
        public IEnumerable<ReservationView> Query(SearchReservationsByFlight query)
        {
            return List.Skip(query.Page * query.PageSize).Take(query.PageSize).Where(c => c.FlightId == query.FlightId);
        }
        public ReservationView Query(FindReservationByLocalizer x)
        {
            return this.List.SingleOrDefault(c => c.Localizer == x.Localizer);
        }
    }
    public class ReservationViewProjection
    {
        ReservationViewRepository Repository;
        public long LastEventId { get; set; }
        public ReservationViewProjection(ReservationViewRepository repo)
        {
            Repository = repo;
        }
        public void On(FlightReservationCreated created)
        {
            Repository.List.Add(new ReservationView { Localizer = created.Localizer, FlightId = created.Localizer });
        }
        public void On(FlightReservationCancelled cancelled)
        {
            var resv = Repository.List.Single(c => c.Localizer == cancelled.Localizer);
            Repository.List.Remove(resv);
        }

    }

    public class SearchReservationsByFlight
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string FlightId { get; set; }
        public SearchReservationsByFlight(string flightId)
        {
            FlightId = flightId;
        }
    }

    public class FindReservationByLocalizer
    {
        public string Localizer { get; }
        public FindReservationByLocalizer(string localizer)
        {

        }
    }
    public class ReservationView
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
