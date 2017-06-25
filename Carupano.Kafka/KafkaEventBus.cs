using System;
using System.Collections;
using System.Linq;

namespace Carupano.Kafka
{
    using System.Collections.Generic;
    using Messaging;
    using Confluent.Kafka;
    using Confluent.Kafka.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Carupano.Persistence;
    using Carupano.Model;
    using Carupano.Messaging.Internal;
    using System.Threading;

    public class KafkaEventBus 
        : IEventBus, IDisposable, IInboundMessageBus, ICommandBus
    {
        Encoding _encoding;
        RouteTable _routes;
        ISerialization _serializer;
        Producer<Null, string> _outbound;
        Consumer<Null, string> _inbound;
        IDictionary<string, object> _config;
        ConsumerPollerThread _thread;
        public event MessageHandler MessageReceived;

        public KafkaEventBus(string server, ISerialization serializer, RouteTable routes)
            : this(new Dictionary<string,object>
            {
                { "bootstrap.servers",server },
                { "enable.auto.commit",false }
            }, serializer, routes)
        {

        }
        public KafkaEventBus(Dictionary<string, object> config, ISerialization serializer, RouteTable routes)
        {
            _serializer = serializer;
            _routes = routes;
            _encoding = Encoding.UTF8;
            _config = config;
            var topics = routes.InboundCommands.Select(c => c.Name).Union(routes.InboundEvents.Select(c => c.Name));

            var seri = new StringSerializer(_encoding);
            var deseri = new StringDeserializer(_encoding);
            var noop = new NullDeserializer();

            _outbound = new Producer<Null, string>(_config, new NullSerializer(), seri);

            var cfg = new Dictionary<string, object>(_config);
            cfg.Add("group.id", Environment.MachineName); //TODO: not right.
            _inbound = new Consumer<Null, string>(cfg, noop, deseri);
            _inbound.OnMessage += (key, val) =>
            {
                OnMessageReceived(val);
            };

            _inbound.Subscribe(topics);
            _thread = new ConsumerPollerThread(_inbound);
            _thread.Start();

        }

        private void OnMessageReceived(Message<Null, string> val)
        {
            if (MessageReceived != null) {
                var data = _serializer.Deserialize(null, _encoding.GetBytes(val.Value));
                Messaging.Message msg = null;
                if (_routes.InboundCommands.Any(c => c.MessageType == data.GetType()))
                    msg = new CommandMessage(val.Offset.Value.ToString(), data);
                else if (_routes.InboundEvents.Any(c => c.MessageType == data.GetType()))
                    msg = new EventMessage(val.Offset.Value.ToString(), val.Offset.Value, data);
                if(msg != null)
                    MessageReceived(msg);
            }
        }
        private void OnEventReceived(Message<Null, string> val)
        {
            if(MessageReceived != null)
            {
                var data = _serializer.Deserialize(null, _encoding.GetBytes(val.Value));
                var evt = new EventMessage(val.Offset.Value.ToString(), val.Offset.Value, data);
                MessageReceived(evt);
            }
        }

        public void Publish(IEnumerable<Tuple<object, long>> evts)
        {
            foreach(var evt in evts)
            {
                Publish(evt.Item1, evt.Item2);
            }
        }

        public void Publish(object evt, long seq)
        {
            Publish(evt);
        }

        public void Publish(object o)
        {
            var route = _routes.OutboundEvents.Single(c => c.MessageType == o.GetType());
            var task = _outbound.ProduceAsync(route.Name, null, _encoding.GetString(_serializer.Serialize(o)));
            task.RunSynchronously();
            var result = task.Result;
        }

        public async Task Send(object cmd)
        {
            var route = _routes.OutboundCommands.Single(c => c.MessageType == cmd.GetType());
            var data = _encoding.GetString(_serializer.Serialize(cmd));
            var result = await _outbound.ProduceAsync(route.Name, null, data);
            _outbound.Flush(TimeSpan.FromSeconds(5));
            
        }

        public void Dispose()
        {
            _thread.Dispose();
            _inbound.Dispose();
            _outbound.Dispose();
        }
    }

    class ConsumerPollerThread : IDisposable
    { 
        CancellationTokenSource _tokenSource;
        CancellationToken _token;
        Consumer<Null, string> _consumer;
        public ConsumerPollerThread(Consumer<Null,string> consumer)
        {
            _consumer = consumer;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (!_token.IsCancellationRequested)
                {
                    _consumer.Poll(100);
                }
            }, _token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }
}
