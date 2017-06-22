using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using CarupanoAirlines.Flight;
using System.Linq;
using Carupano.Persistence;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Carupano.Specs
{
    
    public class when_querying_a_read_model : BaseSpec
    {
        IRepository<ReservationView> Repository;
        public when_querying_a_read_model()
        {
            Repository = Model.Services.GetRequiredService<IRepository<ReservationView>>();
 
        }

        [Fact]
        public async Task gets_single_result()
        {
            await CommandBus.Send(new CreateFlightReservation("passenger", "local", "flight"));
            var result = await Repository.QueryMany(new SearchReservationsByFlight("flight"));
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Single().Localizer.Should().Be("local");
        }
    }
}
