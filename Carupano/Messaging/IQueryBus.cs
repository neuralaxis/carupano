using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Messaging
{
    public interface IQueryBus
    {
        Task<object> Send<TQuery>(TQuery query);
    }
}
