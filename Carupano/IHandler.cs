using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public interface ICommandHandler<T>
    {
        Task Handle(T msg);
    }

    public interface IEventHandler<T>
    {
        Task Handle(T evt);
    }
}
