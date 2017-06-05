using System;

namespace Carupano.Model
{
    public class AggregateIdentifier
    {
        Func<object, string> _expression;
        public AggregateIdentifier(Func<object,string> expr) {
            _expression = expr;
        }
        public string GetId(object instance)
        {
            return _expression(instance);
        }
    }
    
}
