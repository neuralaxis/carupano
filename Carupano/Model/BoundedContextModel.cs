using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.Model
{
    public class BoundedContextModel
    {
        public BoundedContextModel(
            IEnumerable<AggregateModel> aggregates,
            IEnumerable<ProjectionModel> projections = null)
        {
            Aggregates = aggregates;
            Commands = Aggregates.SelectMany(c => c.CommandHandlers.Select(x => x.Command));
            Factories = Aggregates.Select(c => c.FactoryHandler.Command);
            Projections = projections;
            Events = Projections != null ? Projections.SelectMany(c => c.EventHandlers.Select(x => x.Event));
        }

        public IEnumerable<AggregateModel> Aggregates { get; private set; }
        public IEnumerable<ProjectionModel> Projections { get; private set; }
        public IEnumerable<CommandModel> Commands { get; private set; }
        public IEnumerable<CommandModel> Factories { get; private set; }
        public IEnumerable<EventModel> Events { get; private set; }
        
    }
}
