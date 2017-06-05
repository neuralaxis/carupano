using System;

namespace Carupano.Model
{
    public class CommandModel
    {
        public Type TargetType { get; }
        public AggregateCorrelation Correlation { get;  }

        public CommandModel(Type commandType)
            : this(commandType, null)
        {
        }
        public CommandModel(Type commandType, AggregateCorrelation correlation)
        {
            TargetType = commandType;
            Correlation = correlation;
        }
    }
    
}
