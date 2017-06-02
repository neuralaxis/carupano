using System;
using Xunit;

namespace Carupano.Specs
{
    using CarupanoAirlines.Flight;
    using FluentAssertions;
    using System.Linq;

    public class when_configuring_a_context
    {
        Model.BoundedContextModel Model;

        public when_configuring_a_context()
        {
            var builder = new Model.BoundedContextModelBuilder();
            builder.Aggregate<FlightReservation>(cfg =>
            {
                cfg
                .HasId(c => c.Localizer)
                .CreatedBy<CreateFlightReservation>()
                .Executes<CancelFlightReservation>(c => c.Localizer);
            });
            builder.Projection<ReservationList>(cfg =>
            {
                cfg
                    .SubscribeTo<FlightReservationCreated>((p)=>p.On)
                    .SubscribeTo<FlightReservationCancelled>((p) => p.On)
                    .WithState(c => c.State);
            });
            Model = builder.Build();
        }

        [Fact]
        public void builds_aggregate()
        {
            Model.Aggregates.Count().Should().Be(1);
        }
        [Fact]
        public void builds_projection()
        {
            Model.Projections.Should().HaveCount(1);
        }

        [Fact]
        public void builds_command_aggregate_correlation()
        {
            var cmd = Model.Commands.Single(c => c.TargetType == typeof(CancelFlightReservation));
            cmd.Correlation.GetAggregateId(new CancelFlightReservation { Localizer = "1000" }).Should().Be("1000");
        }

        [Fact]
        public void builds_projection_event_handlers()
        {
            Model.Projections.Single().EventHandlers.Should().HaveCount(2);
        }
    }
}
