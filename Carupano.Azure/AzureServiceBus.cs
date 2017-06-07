using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using System.Threading;
using Carupano.Model;

namespace Carupano.Azure
{
    using Messaging;
    using Messaging.Internal;
    public class AzureServiceBus : ICommandBus, IEventBus
    {
        IDictionary<Type, QueueClient> _commands = new Dictionary<Type, QueueClient>();
        IDictionary<Type, TopicClient> _outboundEvent = new Dictionary<Type, TopicClient>();
        IDictionary<Type, SubscriptionClient> _inboundEvents = new Dictionary<Type, SubscriptionClient>();
        ISerialization _serialization;
        Action<object> _command;
        Action<object,long?> _event;
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
                _commands.Add(route.Type, client);
            }
            foreach(var route in routes.OutboundCommands.Where(c=>!_commands.ContainsKey(c.Type)))
            {
                var client = new QueueClient(connectionString, route.Name);
                _commands.Add(route.Type, client);
            }
            foreach(var route in routes.OutboundEvents)
            {
                var client = new TopicClient(connectionString, route.Name);
                _outboundEvent.Add(route.Type, client);
            }
            foreach(var route in routes.InboundEvents)
            {
                var client = new SubscriptionClient(connectionString, route.Name, endpointName);
                client.RegisterMessageHandler(OnInboundEvent);
                _inboundEvents.Add(route.Type, client);
            }
        }

        private Task OnInboundEvent(Message msg, CancellationToken token)
        {
            var evt = Deserialize(msg);
            var seq = msg.UserProperties.ContainsKey("SeqNo") ? Convert.ToInt64(msg.UserProperties["SeqNo"]) : new Nullable<long>();
            _event(evt, seq);
            return Task.FromResult(0);
        }

        public Task OnInboundCommand(Message msg, CancellationToken token)
        {
            var cmd = Deserialize(msg);
            _command(cmd);
            return Task.FromResult(0);
        }

        public void SetEventHandler(Action<object, long?> handler)
        {
            _event = handler;
        }

        public void SetCommandHandler(Action<object> handler)
        {
            _command = handler;
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

        public void Send(object cmd)
        {
            var msg = Serialize(cmd);
            _commands[cmd.GetType()].SendAsync(msg).Wait();
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
