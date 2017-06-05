using System;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;

namespace Carupano.Azure
{
    public class AzureServiceBus : ICommandBus, IEventBus
    {
        QueueClient _commands;
        TopicClient _events;
        Action<object> _command;
        Action<object> _event;
        public AzureServiceBus(string connectionString)
        {
        }
        public void Listen(Action<object, long> handler, long start)
        {
            throw new NotImplementedException();
        }
        public void Listen(Action<object> handler)
        {

        }

        public void Publish<T>(T evt)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(IEnumerable<T> evts)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(T cmd)
        {
            throw new NotImplementedException();
        }
    }
}
