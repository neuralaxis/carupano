using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carupano.Model
{
    public class BoundedContextModelBuilder
    {
        List<IAggregateModelBuilder> _aggregates = new List<IAggregateModelBuilder>();
        List<IProjectionModelBuilder> _projections = new List<IProjectionModelBuilder>();
        public BoundedContextModelBuilder()
        {

        }
        public BoundedContextModelBuilder Aggregate<T>(Action<AggregateModelBuilder<T>> config)
        {
            var builder = new AggregateModelBuilder<T>();
            config(builder);
            _aggregates.Add(builder);
            return this;
        }

        public void Projection<T>(Action<ProjectionModelBuilder<T>> config)
        {
            var builder = new ProjectionModelBuilder<T>();
            config(builder);
            _projections.Add(builder);
        }

        public BoundedContextModel Build()
        {
            return new BoundedContextModel(_aggregates.Select(c=>c.Build()), _projections.Select(c=>c.Build()));
        }
    }
    
}
