using Carupano.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Carupano.Persistence;

namespace Carupano.Configuration
{
    public static class Extensions
    {
        public static BoundedContextModelBuilder UseSqlServerEventStore(this BoundedContextModelBuilder model, string connectionString)
        {
            model.Services(cfg =>
            {
                cfg.AddScoped<IEventStore>((svcs) => new SqlServer.SqlServerEventStore(connectionString, svcs.GetService<ISerialization>()));
            });
            return model;
        }
    }
}
