using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Carupano.Configuration
{
    using Model;
    using System.Linq;
    using System.Reflection;

    public class RepositoryModelBuilder<TModel,TProvider> : IRepositoryModelBuilder
    {
        RepositoryModel _model;
        List<QueryHandlerModel> _queries = new List<QueryHandlerModel>();
        Func<IServiceProvider,TProvider> _factory;
        public RepositoryModelBuilder()
        {

        }
        public RepositoryModelBuilder<TModel, TProvider> RespondsTo<TQuery>(Expression<Func<TProvider,Func<TQuery,TModel>>> func) 
        {
            var convert = func.Body as UnaryExpression;
            var methodCall = (convert.Operand as MethodCallExpression);
            var obj = methodCall.Object as ConstantExpression;
            var method = obj.Value as MethodInfo;

            _queries.Add(new QueryHandlerModel(method, new QueryModel(typeof(TProvider), method.ReturnType, typeof(TQuery))));
            return this;
        }
        public RepositoryModelBuilder<TModel, TProvider> RespondsTo<TQuery>(Expression<Func<TProvider, Func<TQuery, IEnumerable<TModel>>>> func)
        {
            var convert = func.Body as UnaryExpression;
            var methodCall = (convert.Operand as MethodCallExpression);
            var obj = methodCall.Object as ConstantExpression;
            var method = obj.Value as MethodInfo;

            _queries.Add(new QueryHandlerModel(method, new QueryModel(typeof(TProvider), method.ReturnType, typeof(TQuery))));
            return this;
        }
        public RepositoryModelBuilder<TModel, TProvider> AutoConfigure()
        {
            foreach(var method in typeof(TProvider).GetMethods().Where(c=>c.GetParameters().Count() == 1 && !c.IsStatic && c.ReturnType !=typeof(void) && !c.IsConstructor))
            {
                _queries.Add(new QueryHandlerModel(method, new QueryModel(typeof(TProvider), method.ReturnType, method.GetParameters().Single().ParameterType)));
            }
            return this;
        }

        public RepositoryModelBuilder<TModel, TProvider> UseFactory(Func<IServiceProvider,TProvider> factory)
        {
            _factory = factory;
            return this;
        }

        public RepositoryModel Build()
        {
            return new RepositoryModel(typeof(TProvider), new ReadModelModel(typeof(TModel)), _queries, _factory != null ? new Func<IServiceProvider, object>((svcs)=>_factory) : null);
        }
    }
}
