using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Carupano.UnitTests
{
    public static class ReflectionUtils
    {
        public static MethodInfo FindMethod(this Type type, string name, Type argument = null)
        {
            return type.GetMethods().Single(c => c.Name == name && argument == null || c.GetParameters().Any() && c.GetParameters().Single().ParameterType == argument);
        }
    }
}
