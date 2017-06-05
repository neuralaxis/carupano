using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using NSubstitute;

namespace Carupano.UnitTests
{
    using Domains.Airline;
    using Model;
    public class ProjectionModelTests
    {
        IServiceProvider Services;
        ProjectionModel Model;
        public ProjectionModelTests()
        {
            Services = Substitute.For<IServiceProvider>();
            Services.GetService(typeof(ReservationList)).Returns(new ReservationList());
            Model = new ProjectionModel(typeof(ReservationList));
            Model.AddEventHandler(new EventHandlerModel(Model.Type.FindMethod("On", typeof(FlightReservationCreated)), new EventModel(typeof(FlightReservationCreated))));
            Model.AddEventHandler(new EventHandlerModel(Model.Type.FindMethod("On", typeof(FlightReservationCancelled)), new EventModel(typeof(FlightReservationCancelled))));
        }

        [Fact]
        public void adds_event_handlers()
        {
            Model.EventHandlers.Should().HaveCount(2);
        }

        [Fact]
        public void creates_instance()
        {
            var instance = Model.CreateInstance(Services);
            instance.Should().NotBeNull();
        }
    }
}
