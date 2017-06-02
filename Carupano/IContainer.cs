using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano
{
    public interface IContainer : IServiceProvider
    {
        void RegisterInstance(object instance);
        void RegisterFactory<TObj>(Func<IContainer, TObj> factory);
    }
}
