using Carupano.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Runtime
{
    public interface IDispatcher
    {
        void ExecuteCommandHandler(object command);
        void ExecuteEventHandlers(object @event);
    }
}
