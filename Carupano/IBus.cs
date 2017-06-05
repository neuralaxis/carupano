using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public interface IEventBus
    {
        void Publish<T>(T evt);
        void Publish<T>(IEnumerable<T> evts);
        void Listen(Action<object, long> handler, long start);
    }
    public interface ICommandBus
    {
        void Send<T>(T cmd);
        void Listen(Action<object> handler);
    }
}
