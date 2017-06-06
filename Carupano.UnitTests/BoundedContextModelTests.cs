using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace Carupano.UnitTests
{
    using Configuration;
    using Model;
    using System.Linq;

    public class BoundedContextModelTests
    {
        BoundedContextModel Model;
        public BoundedContextModelTests()
        {
            var aggregate = new AggregateModel(typeof(Domains.Airline.FlightReservation));
            Model = new BoundedContextModel(new [] {  aggregate });

        }
        [Fact]
        public void adds_aggregate()
        {
            Model.Aggregates.Count().Should().Be(1);
        }

    }
}
