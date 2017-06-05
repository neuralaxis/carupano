namespace Carupano.Model
{
    public class EventHandlerInstance
    {
        public EventHandlerModel Model { get; set; }
        public object Target { get; set; }
        public EventHandlerInstance(object target, EventHandlerModel model)
        {
            Model = model;
        }
        public bool Handles(PublishedEvent evt)
        {
            return Model.Event.Type == evt.Object.GetType();
        }
        public void Handle(PublishedEvent evt)
        {
            Model.Method.Invoke(Target, new[] { evt.Object });
        }
        public bool Handles(DomainEventInstance instance)
        {
            return Model.Event.Type == instance.Object.GetType();
        }
        public void Handle(DomainEventInstance instance)
        {
            Model.Method.Invoke(Target, new[] { instance.Object });
        }
    }
    
}
