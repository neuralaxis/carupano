using System;
using System.Collections.Generic;
using System.Linq;

namespace Carupano.Model
{
    public class AggregateInstance
    {
        public AggregateModel Model { get; }
        public Object Object { get; }
        public string Id { get { return Model.Identifier.GetId(Object); } }
        public CommandHandlerInstance Factory { get; }
        public IEnumerable<CommandHandlerInstance> CommandHandlers { get; }
        public IEnumerable<EventHandlerInstance> EventHandlers { get; }
        public AggregateInstance(AggregateModel model, object instance)
        {
            Model = model;
            Object = instance;
            Factory = new CommandHandlerInstance(this, model.FactoryHandler);
            CommandHandlers = model.CommandHandlers.Select(c => new CommandHandlerInstance(this, c));
            EventHandlers = model.EventHandlers.Select(c => new EventHandlerInstance(this, c));
        }
        public CommandExecutionResult Execute(CommandInstance cmd)
        {
            CommandHandlerInstance handler = null;
            if (Factory.Handles(cmd))
            {
                handler = Factory;
            }
            else
                handler = CommandHandlers.Single(c => c.Handles(cmd));

            return handler.Execute(cmd);
        }

        public void Handle(DomainEventInstance evt)
        {
            EventHandlers.Single(c=>c.Handles(evt)).Handle(evt);
        }

        public void Apply(IEnumerable<DomainEventInstance> events)
        {
            foreach(var evt in events)
            {
                EventHandlers.Single(c => c.Handles(evt)).Handle(evt);
            }
        }
    }
    
}
