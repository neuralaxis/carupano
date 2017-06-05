using System;

namespace Carupano.Model
{
    public interface IProjectionStateProvider
    {
        long Get(object instance);
        void Set(object instance, long seqNum);
    }
    public class ProjectionAccessorStateProvider  : IProjectionStateProvider
    {
        private Func<object, long> get;
        private Action<object, long> set;

        public ProjectionAccessorStateProvider(Func<object, long> get, Action<object, long> set)
        {
            this.get = get;
            this.set = set;
        }

        public long Get(object instance)
        {
            return get(instance);
        }
        public void Set(object instance, long seq)
        {
            set(instance, seq);
        }
    }
}