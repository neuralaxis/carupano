using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Model
{
    public class ReadModelModel
    {
        public Type Type { get; }
        public ReadModelModel(Type type)
        {
            Type = type;
        }
    }
}
