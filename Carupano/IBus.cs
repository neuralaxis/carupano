using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public interface IEventBus
    {
        void Publish(IEnumerable<Tuple<object, long>> evts);
        void Publish(object evt, long seq);
        void Publish(object o);
        void SetEventHandler(Action<object, long?> handler);
    }

    public interface ICommandBus
    {
        void Send(object cmd);
        void SetCommandHandler(Action<object> handler);
    }

    public class Route
    {
        public string Name { get; }
        public Type Type { get; }
        public Route(string name, Type type)
        {
            Name = name;
            Type = type;
        }
        public bool Matches(Type type)
        {
            return Type == type;
        }
    }

    public class RouteTable
    {
        public IEnumerable<Route> InboundCommands { get; }
        public IEnumerable<Route> OutboundCommands { get; }
        public IEnumerable<Route> InboundEvents { get; }
        public IEnumerable<Route> OutboundEvents { get; }
        public RouteTable(IEnumerable<Route> inboundCmds, IEnumerable<Route> outboundCmds, IEnumerable<Route> inboundEvts, IEnumerable<Route> outboundEvts)
        {
            InboundCommands = inboundCmds;
            OutboundCommands = outboundCmds;
            InboundEvents = inboundEvts;
            OutboundEvents = outboundEvts;
        }
    }
}
