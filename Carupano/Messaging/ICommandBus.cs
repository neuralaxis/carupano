using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{

    public interface ICommandBus
    {
        void Send(object cmd);
        void SetCommandHandler(Action<object> handler);
    }

}
