using System;
using System.Collections.Generic;

namespace Carupano
{
    public interface IEventBus
    {
        void Publish(IEnumerable<Tuple<object, long>> evts);
        void Publish(object evt, long seq);
        void Publish(object o);
        void SetEventHandler(Action<object, long?> handler);
    }
}
