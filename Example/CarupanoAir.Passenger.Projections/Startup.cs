using Carupano;
using Carupano.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarupanoAir.Passenger.Projections
{
    public class Startup
    {
        IConfiguration Config;
        public Startup()
        {
            var cfg = new ConfigurationBuilder();
            cfg.AddInMemoryCollection(new Dictionary<string, string> {
                { "Mongo", "mongodb://localhost/passengers" },
                { "SqlServer", "data source=.\\sqlexpress;initial catalog=CarupanoAir;trusted_connection=true" }
            });
            Config = cfg.Build();
        }
        public void Configure(BoundedContextModelBuilder builder)
        {
            var mongo = Config["Mongo"];
            var sql = Config["SqlServer"];
            builder.WithSqlServerEventStore(sql);
            builder.Projection(
                svcs=>new PassengerList(mongo),
                cfg =>
                {
                    cfg
                    .WithMongoState(mongo)
                    .SubscribesTo<Events.PassengerCreated>(c => c.On)
                    .RespondsTo<Queries.FindPassengerByEmail>();
                });
        }
    }
}
