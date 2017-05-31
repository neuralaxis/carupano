using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Bus
{
    public class InMemoryBus : ICommandBus, IEventBus
    {
        IDispatcher Dispatcher;
        public void Publish<T>(T evt)
        {
            //Find handlers, execute.
        }

        public void Send<T>(T message)
        {
            Dispatcher.ExecuteCommand(message);
            //Find handler, execute
        }
    }
}
