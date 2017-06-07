using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Configuration
{
    public class ReadModelModelBuilder<T>
    {
        public ReadModelModelBuilder<T> RespondsTo<TQuery>()
        {
            return this;
        }
        public ReadModelModelBuilder<T> AutoConfigure()
        {
            return this;
        }
    }
}
