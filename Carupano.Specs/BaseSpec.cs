using Carupano.Configuration;
using Carupano.Model;
using CarupanoAirlines.Flight;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Specs
{
    public class BaseSpec
    {
        protected readonly BoundedContextModel Model;

        public BaseSpec()
        {
            var builder = new BoundedContextModelBuilder();
            builder.UseInMemoryBuses();
            builder.Aggregate<FlightReservation>(cfg =>
            {
                cfg
                .WithId(c => c.Localizer)
                .CreatedBy<CreateFlightReservation>()
                .Executes<CancelFlightReservation>(c => c.Localizer);
            });
            builder.Projection<ReservationList>(cfg =>
            {
                cfg
                    .WithState(c => c.LastEventId)
                    .SubscribesTo<FlightReservationCreated>()
                    .SubscribesTo<FlightReservationCancelled>();
            });
            builder.ReadModel<ReservationListItem, ReservationsRepository>(cfg =>
            {
                cfg.RespondsTo<SearchReservationsByFlight>(c => c.Query);
            });
            Model = builder.Build();
        }

    }
}
