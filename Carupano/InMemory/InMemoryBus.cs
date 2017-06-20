using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.InMemory
{
    using Messaging;
    using Runtime;
    public class InMemoryBus : 
        ICommandBus, 
        IEventBus,
        IInboundMessageBus
    {
        public event MessageHandler MessageReceived;

        public InMemoryBus()
        {
        }

        public void Publish(IEnumerable<Tuple<object, long>> evts)
        {
            foreach(var msg in evts)
            {
                Publish(msg.Item1, msg.Item2);
            }
        }

        public void Publish(object evt, long? seq)
        {
            MessageReceived(new EventMessage(Guid.NewGuid().ToString(), seq.HasValue ? seq.Value : -1, evt));
        }
        public void Publish(object evt, long seq)
        {
            MessageReceived(new EventMessage(Guid.NewGuid().ToString(), seq, evt));
        }

        public void Publish(object o)
        {
            MessageReceived(new EventMessage(Guid.NewGuid().ToString(), -1, o));
        }

        public Task Send(object cmd)
        {
            return Task.Run(() =>
            {
                MessageReceived(new CommandMessage(Guid.NewGuid().ToString(), cmd));
            });
        }
        
        
        
    }
}
