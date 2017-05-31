using System;
using Xunit;

namespace Carupano.Specs
{
    using Domains.Airline;
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
                cfg.HasId(c => c.Id)
                    .CreatedBy<CreateFlightReservation>()
                    .Executes<CancelFlightReservation>(c => c.Id);
            });
            Model = builder.Build();
        }
        [Fact]
        public void can_add_an_aggregate()
        {
            Model.Aggregates.Count().Should().Be(1);
        }

        [Fact]
        public void builds_command_aggregate_correlation()
        {
            var cmd = Model.Commands.Single(c => c.TargetType == typeof(CancelFlightReservation));
            cmd.Correlation.GetAggregateId(new CancelFlightReservation { Id = "1000" }).Should().Be("1000");

        }
    }
}
