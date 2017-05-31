using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Model
{
    public class AggregateModel
    {
        readonly List<CommandHandlerModel> _commandHandlers;
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public CommandHandlerModel FactoryHandler { get; private set; }
        public AggregateIdentifier Identifier { get; private set; }
        public IEnumerable<CommandHandlerModel> CommandHandlers { get { return _commandHandlers; } }

        public AggregateModel(Type type)
        {
            Type = type;
            Name = type.Name;
            _commandHandlers = new List<CommandHandlerModel>();
        }

        public void SetFactoryHandler(CommandHandlerModel model)
        {
            FactoryHandler = model;
        }
        public void SetFactoryHandler(string methodName)
        {
            SetFactoryHandler(new CommandHandlerModel(Type.GetMethods().Single(c => c.Name == methodName)));
        }
        public void SetIdentifier(AggregateIdentifier id)
        {
            Identifier = id;
        }
        public void AddCommandHandler(CommandHandlerModel model)
        {
            _commandHandlers.Add(model);
        }
        public void AddCommandHandler(string methodName)
        {
            AddCommandHandler(new CommandHandlerModel(Type.GetMethods().Single(c => c.Name == methodName)));
        }
        
        public bool IsCreatedBy(CommandModel model)
        {
            return FactoryHandler.Handles(model);
        }
        public bool HandlesCommand(CommandModel cmd)
        {
            return CommandHandlers.Any(c => c.Handles(cmd));
        }

        public AggregateInstance CreateInstance(IServiceProvider container)
        {
            var instance = container.GetService(Type);
            return new AggregateInstance(this, instance);
        }
        
    }

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

        public void Apply(IEnumerable<DomainEventInstance> events)
        {
            foreach(var evt in events)
            {
                EventHandlers.Single(c => c.Handles(evt)).Handle(evt);
            }
        }
    }
    
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
    public class Command
    {
        public Command(object instance)
        {

        }
    }

    public class CommandHandlerModel
    {
        public MethodInfo Method { get; private set; }
        public CommandModel Command { get; private set; }

        public CommandHandlerModel(MethodInfo info, CommandModel model)
        {
            Method = info;
            Command = model;
        }
        public CommandHandlerModel(MethodInfo method)
        {
            Method = method;
            Command = new CommandModel(method.GetParameters().First().ParameterType);
        }
        public bool Handles(CommandModel model)
        {
            return model.TargetType == Command.TargetType;
        }
    }

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
    
    public class CommandInstance
    {
        public CommandModel Model { get; private set; }
        public object Instance { get; private set; }
        public string AggregateId { get { return Model.Correlation.GetAggregateId(Instance); } }

        public CommandInstance(CommandModel model, object instance)
        {
            Model = model;
            Instance = instance;
        }
    }
    public class EventHandlerModel
    {

    }
    public class EventModel
    {

    }
    public class EventHandlerInstance
    {
        public bool Handles(DomainEventInstance instance)
        {
            return false;
        }
        public void Handle(DomainEventInstance instance)
        {

        }
    }
    public class EventMessageInstance
    {
        public EventMessageInstance(object instance)
        {

        }
    }

    public class DomainEventInstance
    {
        public object Object { get; }
        public DomainEventInstance(object instance)
        {
            Object = instance;
        }
    }

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
