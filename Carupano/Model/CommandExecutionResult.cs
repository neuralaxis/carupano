using System;
using System.Collections.Generic;

namespace Carupano.Model
{
    public class CommandExecutionResult
    {
        public IEnumerable<DomainEventInstance> DomainEvents { get; }
        public Exception Exception { get; }
        public bool Success { get { return Exception == null; } }
        public CommandExecutionResult(IEnumerable<DomainEventInstance> events)
        {
            DomainEvents = events;
        }
        public CommandExecutionResult(Exception ex)
        {
            DomainEvents = new List<DomainEventInstance>();
            Exception = ex;
        }
    }
    
}
