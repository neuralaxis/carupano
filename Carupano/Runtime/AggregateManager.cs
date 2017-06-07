using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carupano.Model;

namespace Carupano
{

    public class AggregateManager : IAggregateManager
    {
        readonly IEnumerable<AggregateModel> Aggregates;
        readonly IEnumerable<CommandModel> Factories;
        readonly IEnumerable<CommandModel> Commands;
        readonly IEventStore Store;
        readonly IServiceProvider Services;

        public AggregateManager(IEnumerable<AggregateModel> aggregates, IEventStore store, IServiceProvider svcs)
        {
            Aggregates = aggregates;
            Commands = aggregates.SelectMany(c => c.CommandHandlers.Select(x=>x.Command));
            Factories = aggregates.Select(c => c.FactoryHandler.Command);
            Store = store;
            Services = svcs;
        }

        public CommandExecutionResult ExecuteCommand(object message)
        {
            CommandInstance command;
            AggregateModel aggregate;
            AggregateInstance instance;

            if(Factories.Any(c=>c.TargetType == message.GetType()))
            {
                command = new CommandInstance(
                    Factories.Single(c => c.TargetType == message.GetType()), message);
                aggregate = Aggregates.Single(c => c.IsCreatedBy(command.Model));
                instance = aggregate.CreateInstance(Services);
            }
            else
            {
                command = new CommandInstance(
                    Commands.Single(c => c.TargetType == message.GetType()), message);
                aggregate = Aggregates.Single(c => c.HandlesCommand(command.Model));
                instance = aggregate.CreateInstance(Services);

                var events = Store.Load(aggregate.Name, command.AggregateId).OfType<object>().Select(c => new DomainEventInstance(c));
                instance.Apply(events);
            }

            var result = instance.Execute(command);
            Store.Save(aggregate.Name, instance.Id, result.DomainEvents.Select(c => c.Object));
            return result;
        }
        

    }
}
