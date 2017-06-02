using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public class ProjectionManager
    {
        IEventStore Store;
        IEnumerable<Model.ProjectionInstance> Projections;
        public ProjectionManager(IEventStore store, IEnumerable<Model.ProjectionInstance> projections)
        {
            Store = store;
            Projections = projections;

        }

        public void Start()
        {
            foreach(var proj in Projections)
            {
                Store.Listen((msg, seq) =>
                {
                    proj.Handle(new Model.PublishedEvent(msg, seq));
                    proj.SetState(seq);
                }, proj.GetState());
            }
        }
    }
}
