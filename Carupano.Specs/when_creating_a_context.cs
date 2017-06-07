using System;
using Xunit;

namespace Carupano.Specs
{
    using CarupanoAirlines.Flight;
    using FluentAssertions;
    using System.Linq;
    using Carupano.Configuration;
    using Model;
    public class when_configuring_a_context : BaseSpec
    {
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

        [Fact]
        public void builds_query_models()
        {
            Model.Queries.Should().HaveCount(1);
        }

        [Fact]
        public void builds_read_models()
        {
            Model.ReadModels.Should().HaveCount(1);
        }
    }
}
