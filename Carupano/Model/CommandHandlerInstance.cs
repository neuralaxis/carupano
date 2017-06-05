using System.Collections.Generic;

namespace Carupano.Model
{
    public class CommandHandlerInstance
    {
        public CommandHandlerModel Model { get; }
        public AggregateInstance Instance { get; }

        public CommandHandlerInstance(AggregateInstance aggregate, CommandHandlerModel model)
        {
            Model = model;
            Instance = aggregate;
        }
        public bool Handles(CommandInstance instance)
        {
            return Model.Handles(instance.Model);
        }

        internal CommandExecutionResult Execute(CommandInstance cmd)
        {
            var evt = Model.Method.Invoke(Instance.Object, new[] { cmd.Instance });
            var list = new List<DomainEventInstance>();
            list.Add(new DomainEventInstance(evt));
            return new CommandExecutionResult(list);
        }
    }
    
}
