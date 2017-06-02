using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Model
{
    public class ProjectionModel
    {
        List<EventHandlerModel> _events = new List<EventHandlerModel>();
        List<QueryHandlerModel> _queries = new List<QueryHandlerModel>();
        public Type Type { get; }
        public IEnumerable<EventHandlerModel> EventHandlers { get { return _events; } }
        public IEnumerable<QueryHandlerModel> QueryHandlers { get { return _queries; } }
        public ProjectionStateAccessor StateAccessor { get; private set; }
        public ProjectionModel(Type projection)
        {
            Type = projection;
        }

        public void AddEventHandler(EventHandlerModel eventHandlerModel)
        {
            _events.Add(eventHandlerModel);
        }
        public void AddQueryHandler(QueryHandlerModel model)
        {
            _queries.Add(model);
        }
        public void SetStateAccessor(ProjectionStateAccessor accessor)
        {
            StateAccessor = accessor;
        }

        public bool IsSubscribedToEvent(EventModel model)
        {
            return this.EventHandlers.Any(c => c.Event.Type == model.Type); //TODO: equality!?
        }
        public ProjectionInstance CreateInstance(IServiceProvider svcs)
        {
            var inst = svcs.GetService(Type);
            return new ProjectionInstance(this, inst);
        }
    }

    public class ProjectionInstance
    {
        public ProjectionModel Model { get; }
        public object Object { get; }
        public IEnumerable<EventHandlerInstance> EventHandlers { get; }
        public IEnumerable<QueryHandlerInstance> QueryHandlers { get; }
        public ProjectionInstance(ProjectionModel model, object instance)
        {
            Model = model;
            Object = instance;
            EventHandlers = model.EventHandlers.Select(c => new EventHandlerInstance(this.Object, c));
            QueryHandlers = model.QueryHandlers.Select(c => new QueryHandlerInstance(this.Object, c));
        }

        public bool Handles(PublishedEvent evt)
        {
            return EventHandlers.Any(c => c.Handles(evt));
        }

        public void Handle(PublishedEvent evt)
        {
            EventHandlers.Single(c => c.Handles(evt)).Handle(evt);
        }

        public bool Handles(QueryInstance query)
        {
            return false;
        }

        public void SetState(long seqNum)
        {
            Model.StateAccessor.Set(Object, seqNum);
        }
        public long GetState()
        {
            return Model.StateAccessor.Get(Object);
        }
    }
}
