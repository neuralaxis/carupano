using System;
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
            var get = new Func<object, string>((obj) => {
                var expr = accessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                return (string)prop.GetValue(obj);
            });
            var set = new Action<object, object>((obj,value) => {
                var expr = accessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                prop.SetValue(obj, value);
            });
            return this;
        }
        public ProjectionModelBuilder<T> SubscribeTo<TEvent>(Expression<Func<T,Action<TEvent>>> handler)
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
        public ProjectionModel Build()
        {
            return _model;
        }
    }
}