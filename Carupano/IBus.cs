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
    }
    public interface ICommandBus
    {
        void Send<T>(T cmd);
    }
}
