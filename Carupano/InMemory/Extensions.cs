using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Carupano
{
    using Persistence;
    using Carupano.Messaging;
    using Configuration;
    using InMemory;
    public static class Extensions
    {
        public static BoundedContextModelBuilder UseInMemoryEventStore(this BoundedContextModelBuilder builder)
        {
            builder.Services(cfg =>
            {
                cfg.AddSingleton<IEventStore>(new InMemoryEventStore());
            });
            return builder;
        }
        public static BoundedContextModelBuilder UseInMemoryBuses(this BoundedContextModelBuilder builder)
        {
            builder.Services(cfg =>
            {
                var bus = new InMemoryBus();
                cfg.AddSingleton<ICommandBus>(bus);
                cfg.AddSingleton<IEventBus>(bus);
                cfg.AddSingleton<IInboundMessageBus>(bus);
            });
            return builder;
        }
    }
}
