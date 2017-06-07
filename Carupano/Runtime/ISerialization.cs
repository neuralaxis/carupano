using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano
{
    public interface ISerialization
    {
        byte[] Serialize(object o);
        object Deserialize(Type type, byte[] bytes);
    }
}
