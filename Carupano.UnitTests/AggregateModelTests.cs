using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using FluentAssertions;
using System.Reflection;

namespace Carupano.UnitTests
{
    using Configuration;
    using Model;
    using NSubstitute;

    public class AggregateModelTests
    {
        AggregateModel Model;

        public AggregateModelTests()
        {
            Model = new AggregateModel(typeof(Domains.Airline.FlightReservation));
            Model.SetFactoryHandler(new CommandHandlerModel(Model.Type.GetMethods().Single(c => c.Name == "Create")));
            Model.AddCommandHandler(new CommandHandlerModel(Model.Type.GetMethods().Single(c => c.Name == "Cancel")));
        }

        [Fact]
        public void sets_name()
        {
            Model.Name.Should().NotBeNullOrEmpty();
            Model.Name.Should().Be("FlightReservation");
        }

        [Fact]
        public void sets_factory_method()
        {
            Model.FactoryHandler.Should().NotBeNull();
            Model.FactoryHandler.Method.Name.Should().Be("Create");
        }

        [Fact]
        public void handles_factory_command()
        {
            Model.FactoryHandler.Handles(new CommandModel(typeof(Domains.Airline.CreateFlightReservation))).Should().Be(true);
        }

        [Fact]
        public void creates_instance()
        {
            var svc = Substitute.For<IServiceProvider>();
            var obj = new Domains.Airline.FlightReservation();
            svc.GetService(typeof(Domains.Airline.FlightReservation)).Returns(obj);

            var instance = Model.CreateInstance(svc);

            instance.Should().NotBeNull();
            instance.Model.Should().Be(Model);
            instance.Object.Should().Be(obj);
            instance.Factory.Should().NotBeNull();
            instance.CommandHandlers.Should().NotBeNull();
        }
        

    }
}
