using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carupano.Configuration
{
    using Model;
    public class BoundedContextModelBuilder
    {
        IServiceCollection _services;
        List<IAggregateModelBuilder> _aggregates = new List<IAggregateModelBuilder>();
        List<IProjectionModelBuilder> _projections = new List<IProjectionModelBuilder>();

        public BoundedContextModelBuilder()
        {
            _services = new ServiceCollection();
        }
        public BoundedContextModelBuilder Aggregate<T>(Action<AggregateModelBuilder<T>> config)
        {
            var builder = new AggregateModelBuilder<T>();
            config(builder);
            _aggregates.Add(builder);
            _services.AddScoped(typeof(T));
            return this;
        }

        public void Projection<T>(Action<ProjectionModelBuilder<T>> config)
        {
            var builder = new ProjectionModelBuilder<T>();
            config(builder);
            _projections.Add(builder);
            _services.AddScoped(typeof(T));
        }

        public void Projection<T>(Func<IServiceProvider,T> factory, Action<ProjectionModelBuilder<T>> config) where T: class
        {
            var builder = new ProjectionModelBuilder<T>();
            config(builder);
            _projections.Add(builder);
            _services.AddScoped(factory);
        }

        public void ReadModel<T>(Action<ReadModelModelBuilder<T>> config)
        {
            var builder = new ReadModelModelBuilder<T>();
            config(builder);
            
        }

        public void Services(Action<IServiceCollection> cfg)
        {
            cfg(_services);
        }

        public BoundedContextModel Build()
        {
            var projections = _projections.Select(c => c.Build());
            var aggregates = _aggregates.Select(c => c.Build());
            return new BoundedContextModel(
                aggregates,
                projections,
                _services.BuildServiceProvider());
        }
    }
    
}
