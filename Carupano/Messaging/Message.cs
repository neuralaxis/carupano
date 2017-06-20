using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Messaging
{
    public abstract class Message
    {
        public string Id { get; protected set; }
        public object Payload { get; protected set; }
    }
    public class CommandMessage : Message
    {
        public CommandMessage(string id, object payload)
        {
            this.Id = id;
            this.Payload = payload;
        }
    }
    public class EventMessage : Message
    {
        public long SequenceNo { get; private set; }
        public EventMessage(string id, long seq, object payload)
        {
            this.Id = id;
            this.SequenceNo = seq;
            this.Payload = payload;
        }
    }
}
