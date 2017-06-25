using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using System.Threading;
using Carupano.Model;

namespace Carupano.Azure
{
    using Messaging.Internal;
    public class AzureServiceBus : Messaging.ICommandBus, Messaging.IEventBus, Messaging.IInboundMessageBus
    {
        IDictionary<Type, QueueClient> _commands = new Dictionary<Type, QueueClient>();
        IDictionary<Type, TopicClient> _outboundEvent = new Dictionary<Type, TopicClient>();
        IDictionary<Type, SubscriptionClient> _inboundEvents = new Dictionary<Type, SubscriptionClient>();
        ISerialization _serialization;

        public event Messaging.MessageHandler MessageReceived;

        public AzureServiceBus(string connectionString, 
            string endpointName, 
            ISerialization serialization,
            RouteTable routes)
        {
            _serialization = serialization;
            foreach(var route in routes.InboundCommands)
            {
                var client = new QueueClient(connectionString, route.Name);
                client.RegisterMessageHandler(OnInboundCommand);
                _commands.Add(route.MessageType, client);
            }
            foreach(var route in routes.OutboundCommands.Where(c=>!_commands.ContainsKey(c.MessageType)))
            {
                var client = new QueueClient(connectionString, route.Name);
                _commands.Add(route.MessageType, client);
            }
            foreach(var route in routes.OutboundEvents)
            {
                var client = new TopicClient(connectionString, route.Name);
                _outboundEvent.Add(route.MessageType, client);
            }
            foreach(var route in routes.InboundEvents)
            {
                var client = new SubscriptionClient(connectionString, route.Name, endpointName);
                client.RegisterMessageHandler(OnInboundEvent);
                _inboundEvents.Add(route.MessageType, client);
            }
        }

        private Task OnInboundEvent(Message msg, CancellationToken token)
        {
            var evt = Deserialize(msg);
            var id = msg.MessageId;
            var seq = msg.UserProperties.ContainsKey("SeqNo") ? Convert.ToInt64(msg.UserProperties["SeqNo"]) : new Nullable<long>();
            OnMessageReceived(new Messaging.EventMessage(msg.MessageId, seq.HasValue ? seq.Value : -1, evt));
            return Task.FromResult(0);
        }

        public Task OnInboundCommand(Message msg, CancellationToken token)
        {
            var cmd = Deserialize(msg);
            var id = msg.MessageId;
            OnMessageReceived(new Messaging.CommandMessage(id, cmd));
            return Task.FromResult(0);
        }
        
        private void OnMessageReceived(Messaging.Message msg)
        {
            MessageReceived(msg);
        }
        private object Deserialize(Message msg)
        {
            var type = msg.UserProperties["ClrType"] as string;
            var inst = _serialization.Deserialize(Type.GetType(type), msg.Body);
            return inst;
        }

        private Message Serialize(object o)
        {
            var type = o.GetType().Name;
            var data = _serialization.Serialize(o);
            var msg = new Message(data);
            msg.UserProperties["ClrType"] = msg;
            return msg;
        }

        public async Task Send(object cmd)
        {
            var msg = Serialize(cmd);
            await _commands[cmd.GetType()].SendAsync(msg);
        }

        public void Publish(IEnumerable<Tuple<object, long>> evts)
        {
            foreach(var evt in evts)
            {
                Publish(evt.Item1, evt.Item2);
            }
        }

        public void Publish(object o)
        {
            Publish(o);
        }
        public void Publish(object o, long seq)
        {
            Publish(o, seq);
        }
        public void Publish(object evt, long? seq = null)
        {
            var msg = Serialize(evt);
            if(seq.HasValue)
                msg.UserProperties["SeqNo"] = seq;
            _outboundEvent[evt.GetType()].SendAsync(msg);
        }

    }
    
}
