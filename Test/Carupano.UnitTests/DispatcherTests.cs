using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using NSubstitute;

namespace Carupano.UnitTests
{
    using Model;
    using System.Collections;
    using System.Linq;

    public class DispatcherTests
    {
        Dispatcher Dispatcher;
        IEventStore Events;
        IServiceProvider Services;
        public DispatcherTests()
        {
            Events = Substitute.For<IEventStore>();
            Services = Substitute.For<IServiceProvider>();
            Services.GetService(typeof(Domains.Airline.FlightReservation)).Returns(new Domains.Airline.FlightReservation());
            var resv = new AggregateModel(typeof(Domains.Airline.FlightReservation));
            resv.SetFactoryHandler("Create");
            resv.AddCommandHandler("Cancel");
            Dispatcher = new Dispatcher(new BoundedContextModel(new[] {resv}), Events, Services);
        }
        [Fact]
        public void creates_aggregate()
        {
            var result = Dispatcher.ExecuteCommand(new Domains.Airline.CreateFlightReservation("123", "ABC", "DEF"));
            result.Success.Should().BeTrue();
            result.DomainEvents.Count().Should().Be(1);
            result.DomainEvents.First().Object.Should().BeOfType<Domains.Airline.FlightReservationCreated>();
        }
    }
}
