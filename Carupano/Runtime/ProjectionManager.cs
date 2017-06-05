using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public class ProjectionManager
    {
        IEventBus Store;
        IEnumerable<Model.ProjectionInstance> Projections;
        public ProjectionManager(IEventBus store, IEnumerable<Model.ProjectionInstance> projections)
        {
            Store = store;
            Projections = projections;

        }

        public void Start()
        {
            foreach(var proj in Projections)
            {
                Store.SetEventHandler((msg, seq) =>
                {
                    var evt = new Model.PublishedEvent(msg, seq.Value);
                    if (proj.Handles(evt))
                    {
                        proj.Handle(evt);
                        proj.SetState(seq.Value);
                    }
                });
            }
        }
    }
}
