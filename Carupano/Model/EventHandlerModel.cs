using System.Reflection;

namespace Carupano.Model
{
    public class EventHandlerModel
    {
        public MethodInfo Method { get; }
        public EventModel Event { get; }
        public EventHandlerModel(MethodInfo method, EventModel evt)
        {
            Method = method;
            Event = evt;
        }
    }
    
}
