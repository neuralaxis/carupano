using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Carupano.Model
{
    public class ProjectionModelBuilder<T> : IProjectionModelBuilder
    {
        ProjectionModel _model;

        public ProjectionModelBuilder() {
            _model = new ProjectionModel(typeof(T));
        }
        public ProjectionModelBuilder<T> WithState(Expression<Func<T, long>> accessor)
        {
            var get = new Func<object, long>((obj) => {
                var expr = accessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                return (long)prop.GetValue(obj);
            });
            var set = new Action<object, long>((obj,value) => {
                var expr = accessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                prop.SetValue(obj, value);
            });
            _model.SetStateAccessor(new ProjectionStateAccessor(get, set));
            return this;
        }

        public ProjectionModelBuilder<T> RespondsTo<TQuery>()
        {
            var method = FindMethodByParameter(typeof(TQuery));
            _model.AddQueryHandler(new QueryHandlerModel(method, new QueryModel(typeof(T), method.ReturnType, typeof(TQuery))));
            return this;
        }

        public ProjectionModelBuilder<T> SubscribesTo<TEvent>(Expression<Func<T,Action<TEvent>>> handler)
        {
            //TODO: may have to keep list of EventModels so there aren't multiple for the same event type, or make it a value object.
            var model = new EventModel(typeof(TEvent));
            var convert = handler.Body as UnaryExpression;
            var methodCall = (convert.Operand as MethodCallExpression);
            var obj = methodCall.Object as ConstantExpression;
            var method = obj.Value as MethodInfo;

            _model.AddEventHandler(new EventHandlerModel(method, model));
            return this;
        }

        public ProjectionModelBuilder<T> SubscribesTo<TEvent>()
        {
            var model = new EventModel(typeof(TEvent));
            _model.AddEventHandler(new EventHandlerModel(FindMethodByParameter(typeof(TEvent)), model));
            return this;
        }

        private MethodInfo FindMethodByParameter(Type param)
        {
            return _model.Type.GetMethods().Single(c => c.GetParameters().Count() == 1 && c.GetParameters().First().ParameterType == param);
        }

        public ProjectionModel Build()
        {
            return _model;
        }
    }
}