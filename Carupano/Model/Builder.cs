using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Carupano.Model
{
    public class BoundedContextModelBuilder
    {
        List<IAggregateModelBuilder> _aggregates = new List<IAggregateModelBuilder>();
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

        public BoundedContextModel Build()
        {
            return new BoundedContextModel(_aggregates.Select(c=>c.Model));
        }
    }

    interface IAggregateModelBuilder
    {
        AggregateModel Model { get; }
    }
    public class AggregateModelBuilder<T> : IAggregateModelBuilder
    {
        AggregateModel _model;
        AggregateModel IAggregateModelBuilder.Model { get { return _model; } }
        public AggregateModelBuilder()
        {
            _model = new AggregateModel(typeof(T));   

        }
        
        public AggregateModelBuilder<T> Executes<TCommand>(Expression<Func<TCommand,string>> correlationAccessor)
        {
            var id = new Func<object, string>((obj) => {
                var expr = correlationAccessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                return (string)prop.GetValue(obj);
            });
            _model.AddCommandHandler(new CommandHandlerModel(FindMethodByParameter(typeof(TCommand)), new CommandModel(typeof(TCommand), new AggregateCorrelation(id))));
            return this;
        }
        public AggregateModelBuilder<T> CreatedBy<TCommand>()
        {
            _model.SetFactoryHandler(new CommandHandlerModel(FindMethodByParameter(typeof(TCommand))));
            return this;
        }

        public AggregateModelBuilder<T> HasId(Expression<Func<T,string>> idAccessor)
        {
            _model.SetIdentifier(new AggregateIdentifier((obj)=> {
                var expr = idAccessor.Body as MemberExpression;
                var prop = expr.Member as PropertyInfo;
                return (string)prop.GetValue(obj);
            }));
            return this;
        }
        private MethodInfo FindMethodByParameter(Type param)
        {
            return _model.Type.GetMethods().Single(c => c.GetParameters().Count() == 1 && c.GetParameters().First().ParameterType == param);
        }

    }
    
}
