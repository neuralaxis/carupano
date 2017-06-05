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
        readonly List<EventHandlerModel> _eventHandlers;
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public CommandHandlerModel FactoryHandler { get; private set; }
        public AggregateIdentifier Identifier { get; private set; }
        public IEnumerable<CommandHandlerModel> CommandHandlers { get { return _commandHandlers; } }
        public IEnumerable<EventHandlerModel> EventHandlers {  get { return _eventHandlers; } }
        public AggregateModel(Type type)
        {
            Type = type;
            Name = type.Name;
            _commandHandlers = new List<CommandHandlerModel>();
            _eventHandlers = new List<EventHandlerModel>();
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
        public void AddEventHandler(EventHandlerModel model)
        {
            _eventHandlers.Add(model);
        }
        public bool IsCreatedBy(CommandModel model)
        {
            return FactoryHandler.Handles(model);
        }
        public bool HandlesCommand(CommandModel cmd)
        {
            return CommandHandlers.Any(c => c.Handles(cmd));
        }
        public bool IsSubscribedToEvent(EventModel model)
        {
            return EventHandlers.Any(c => c.Event.Type == model.Type);
        }

        public AggregateInstance CreateInstance(IServiceProvider container)
        {
            var instance = container.GetService(Type);
            return new AggregateInstance(this, instance);
        }
        
    }
    
}
