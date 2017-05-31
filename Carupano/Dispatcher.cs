using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carupano.Model;

namespace Carupano
{
    interface IDispatcher
    {
        CommandExecutionResult ExecuteCommand(object command);
        void PublishEvent(object @event);
    }

    public class Dispatcher : IDispatcher
    {
        BoundedContextModel Model;
        IEventStore Store;
        IServiceProvider Services;
        public Dispatcher(BoundedContextModel model, IEventStore store, IServiceProvider svcs)
        {
            Model = model;
            Store = store;
            Services = svcs;
        }

        public CommandExecutionResult ExecuteCommand(object message)
        {
            CommandInstance command;
            AggregateModel aggregate;
            AggregateInstance instance;

            if(Model.Factories.Any(c=>c.TargetType == message.GetType()))
            {
                command = new CommandInstance(
                    Model.Factories.Single(c => c.TargetType == message.GetType()), message);
                aggregate = Model.Aggregates.Single(c => c.IsCreatedBy(command.Model));
                instance = aggregate.CreateInstance(Services);
            }
            else
            {
                command = new CommandInstance(
                    Model.Commands.Single(c => c.TargetType == message.GetType()), message);
                aggregate = Model.Aggregates.Single(c => c.HandlesCommand(command.Model));
                instance = aggregate.CreateInstance(Services);

                var events = Store.Load(aggregate.Name, command.AggregateId).OfType<object>().Select(c => new DomainEventInstance(c));
                instance.Apply(events);
            }

            var result = instance.Execute(command);
            Store.Save(aggregate.Name, instance.Id, result.DomainEvents.Select(c => c.Object));
            return result;
        }

        public void PublishEvent(object @event)
        {
        }
    }
}
