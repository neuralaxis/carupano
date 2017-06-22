using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Carupano.Configuration
{
    using Carupano.Messaging;
    using Model;
    using Persistence;
    using Runtime;

    public class BoundedContextModelBuilder
    {
        IServiceCollection _services;
        List<IAggregateModelBuilder> _aggregates = new List<IAggregateModelBuilder>();
        List<IProjectionModelBuilder> _projections = new List<IProjectionModelBuilder>();
        List<IRepositoryModelBuilder> _repos = new List<IRepositoryModelBuilder>();

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

        public void ReadModel<TModel, TProvider>(Action<RepositoryModelBuilder<TModel,TProvider>> config)
        {
            var builder = new RepositoryModelBuilder<TModel, TProvider>();
            config(builder);
            _repos.Add(builder);
        }

        public void Services(Action<IServiceCollection> cfg)
        {
            cfg(_services);
        }

        public BoundedContextModel Build()
        {
            var projections = _projections.Select(c => c.Build());
            var aggregates = _aggregates.Select(c => c.Build());
            var repositories = _repos.Select(c => c.Build());
            
            foreach(var repo in repositories)
            {
                if (!_services.Any(c => c.ServiceType.IsAssignableFrom(repo.Type)))
                    if (repo.Factory != null) _services.AddScoped(repo.Type, repo.Factory);
                    else _services.AddScoped(repo.Type);

                _services.AddScoped(repo.GenericServiceType, repo.GetRepositoryServiceFactory());
            }
            return new BoundedContextModel(
                aggregates,
                projections,
                repositories,
                _services.BuildServiceProvider());
        }
    }
    
}
