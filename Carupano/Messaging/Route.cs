using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Messaging.Internal
{

    public class Route
    {
        public string Name { get; }
        public Type MessageType { get; }
        public Route(string name, Type type)
        {
            Name = name;
            MessageType = type;

        }
        public bool Matches(Type message)
        {
            return MessageType == message;
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
