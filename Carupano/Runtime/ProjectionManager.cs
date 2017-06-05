using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public class ProjectionManager
    {
        IEventBus Bus;
        IEventStore Store;
        IEnumerable<Model.ProjectionInstance> Projections;
        public ProjectionManager(IEventStore store, IEventBus bus, IEnumerable<Model.ProjectionInstance> projections)
        {
            Store = store;
            Bus = bus;
            Projections = projections;

        }

        public void Start()
        {
            foreach(var proj in Projections)
            {
                foreach(var msg in Store.Load(proj.GetState()))
                {
                    var evt = new Model.PublishedEvent(msg.Event, msg.SequenceNo);
                    if(proj.Handles(evt))
                    {
                        proj.Handle(evt);
                    }
                }
                //TODO: events might come while it's reading, so we need to set teh event handler
                //first and accumulate while building projection.
                Bus.SetEventHandler((msg, seq) =>
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
