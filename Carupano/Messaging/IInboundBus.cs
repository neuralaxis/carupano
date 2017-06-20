using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Messaging
{
    public delegate void MessageHandler(Message msg);
    public interface IInboundMessageBus
    {
        event MessageHandler MessageReceived;
    }
}
