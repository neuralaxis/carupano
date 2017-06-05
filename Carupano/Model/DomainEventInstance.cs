namespace Carupano.Model
{
    public class DomainEventInstance
    {
        public object Object { get; }
        public DomainEventInstance(object instance)
        {
            Object = instance;
        }
    }
    
}
