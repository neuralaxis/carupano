using System;

namespace Carupano.Model
{
    public class EventModel
    {
        public Type Type { get; }
        public EventModel(Type type)
        {
            Type = type;
        }
    }
    
}
