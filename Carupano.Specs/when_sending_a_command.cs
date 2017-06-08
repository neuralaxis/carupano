using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CarupanoAirlines.Flight;
namespace Carupano.Specs
{
    using Messaging;
    using System.Threading.Tasks;

    public class when_sending_a_command :BaseSpec
    {

        [Fact]
        public async Task does_not_throw_an_exception()
        {
            await CommandBus.Send(new CreateFlightReservation("test", "test", "test"));
        }

        [Fact]
        public async Task published_events_are_received_by_projections()
        {

        }
    }
}
