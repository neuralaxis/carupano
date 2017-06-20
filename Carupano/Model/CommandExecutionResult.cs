using System;
using System.Collections.Generic;

namespace Carupano.Model
{
    public class CommandExecutionResult
    {
        public IEnumerable<DomainEventInstance> DomainEvents { get; }
        public object Output { get; }
        public Exception Exception { get; }
        public bool Success { get { return Exception == null; } }
        public bool HasOutput {  get { return Output != null; } }

        public CommandExecutionResult(IEnumerable<DomainEventInstance> events, object output = null)
        {
            DomainEvents = events;
            Output = output;
        }

        public CommandExecutionResult(Exception ex)
        {
            DomainEvents = new List<DomainEventInstance>();
            Exception = ex;
        }
    }
    
}
