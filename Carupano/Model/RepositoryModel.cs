using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
namespace Carupano.Model
{
    using System.Threading.Tasks;
    using Persistence;
    using Microsoft.Extensions.DependencyInjection;

    public class RepositoryModel
    {
        public Type Type {get;}
        public Type ServiceType { get; }
        public Type GenericServiceType { get; }
        public ReadModelModel Model { get; }
        public IEnumerable<QueryHandlerModel> QueryHandlers { get; }
        public Func<IServiceProvider, object> Factory;

        public RepositoryModel(Type repoType, ReadModelModel model, IEnumerable<QueryHandlerModel> queries, Func<IServiceProvider, object> factory = null)
        {
            Type = repoType;
            Model = model;
            QueryHandlers = queries;
            ServiceType = typeof(RepositoryService<>).MakeGenericType(model.Type);
            GenericServiceType = ServiceType.GetInterfaces().First();
            Factory = factory;
        }

        public Func<IServiceProvider,object> GetRepositoryServiceFactory()
        {
            return new Func<IServiceProvider, object>((svcs) =>
            {
                object repo = null;
                if (Factory == null)
                    repo = svcs.GetRequiredService(Type);
                else
                    repo = Factory(svcs);
                var inst = new RepositoryInstance(this, repo);
                return Activator.CreateInstance(ServiceType, inst, inst.QueryHandlers.Select(c => c.Model.Query));
            });
        }
    }

    public class RepositoryInstance 
    {
        public object Object { get; }
        public RepositoryModel Model { get; }
        public IEnumerable<QueryHandlerInstance> QueryHandlers { get; }
        public RepositoryInstance(RepositoryModel model, object instance)
        {
            Object = instance;
            Model = model;
            QueryHandlers = Model.QueryHandlers.Select(c => new QueryHandlerInstance(instance, model.QueryHandlers.Single(x => x.Query.IsSameAs(c.Query))));
        }
        public QueryResponse HandleQuery(QueryInstance instance)
        {
            return QueryHandlers.Single(c => c.Handles(instance)).Handle(instance);
        }
    }

    public class RepositoryService<T> : IRepository<T>
    {
        RepositoryInstance _instance;
        IEnumerable<QueryModel> Queries { get; }
        public RepositoryService(RepositoryInstance instance, IEnumerable<QueryModel> query)
        {
            _instance = instance;
            Queries = instance.Model.QueryHandlers.Select(c => c.Query);
        }
        public Task<IEnumerable<T>> QueryMany<TQuery>(TQuery query)
        {
            return Execute<IEnumerable<T>>(query);
        }

        public Task<T> QuerySingle<TQuery>(TQuery query)
        {
            return Execute<T>(query);
        }

        private Task<TResult> Execute<TResult>(object query)
        {
            var task = new Task<TResult>(() =>
            {
                var result = _instance.HandleQuery(new QueryInstance(query, Queries.Single(c => c.Type == query.GetType())));
                return (TResult)result.Result;
            });
            task.Start();
            return task;

        }
    }

}
