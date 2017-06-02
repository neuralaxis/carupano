using System;

namespace Carupano.Model
{
    internal class ProjectionStateAccessor
    {
        private Func<object, long> get;
        private Action<object, long> set;

        public ProjectionStateAccessor(Func<object, long> get, Action<object, long> set)
        {
            this.get = get;
            this.set = set;
        }

        public long GetState(object instance)
        {
            return get(instance);
        }
        public void SetState(object instance, long seq)
        {
            set(instance, seq);
        }
    }
}