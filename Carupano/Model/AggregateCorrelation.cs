using System;

namespace Carupano.Model
{
    public class AggregateCorrelation
    {
        Func<object, string> _expr;
        public AggregateCorrelation(Func<object,string> expr)
        {
            _expr = expr;
        }
        public string GetAggregateId(object instance)
        {
            return _expr(instance);
        }
    }
    
}
