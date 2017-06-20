using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Messaging
{
    public interface ICommandBus
    {
        Task Send(object cmd);
    }

}
