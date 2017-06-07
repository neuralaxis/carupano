using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Carupano.Configuration
{
    using Messaging;
    using Messaging.Internal;
    public static class Extensions
    {
        public static BoundedContextModelBuilder WithAzureServiceBus(this BoundedContextModelBuilder model, string connectionString)
        {
            model.Services(cfg =>
            {
                Func<IServiceProvider, Azure.AzureServiceBus> factory = (svcs) =>
                {
                    //TODO: endpoint name
                    return new Azure.AzureServiceBus(connectionString, String.Empty, svcs.GetService<ISerialization>(), svcs.GetService<RouteTable>());
                };
                cfg.AddScoped<ICommandBus>(factory);
                cfg.AddScoped<IEventBus>(factory);
            });
            return model;
        }
    }
}
