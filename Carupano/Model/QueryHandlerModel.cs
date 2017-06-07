using System;
using System.Reflection;

namespace Carupano.Model
{
    public class QueryHandlerModel
    {
        public MethodInfo Method { get; }
        public QueryModel Query { get; }

        public QueryHandlerModel(MethodInfo methodInfo, QueryModel query)
        {
            Method = methodInfo;
            Query = query;
        }

        public bool Handles(QueryModel model)
        {
            return Query.Type == model.Type && Query.ResponseType == model.ResponseType;
        }
    }

    public class QueryHandlerInstance
    {
        public QueryHandlerModel Model { get; }
        public object Instance { get; }
        public QueryHandlerInstance(object instance, QueryHandlerModel model)
        {
            Instance = instance;
            Model = model;
        }

        public bool Handles(QueryInstance instance)
        {
            return Model.Query.IsSameAs(instance.Model);
        }
        public QueryResponse Handle(QueryInstance instance)
        {
            var result = Model.Method.Invoke(Instance, new[] { instance.Object });
            return new QueryResponse(instance, this, result);
        }
    }

    public class QueryResponse
    {
        public QueryInstance Query { get; }
        public QueryHandlerInstance Handler { get; }
        public object Result { get; }
        public QueryResponse(QueryInstance query, QueryHandlerInstance handler, object result)
        {
            Query = query;
            Handler = handler;
            Result = result;
        }
    }

    public class QueryModel
    {
        public Type TargetType { get; }
        public Type Type { get; }
        public Type ResponseType { get; }
        public QueryModel(Type targetType, Type responseType, Type type)
        {
            TargetType = targetType;
            ResponseType = responseType;
            Type = type;
        }

        public bool IsSameAs(QueryModel query)
        {
            return TargetType == query.TargetType
                 && ResponseType == query.ResponseType
                 && Type == query.Type;
        }
    }
    public class QueryInstance
    {
        public object Object { get; }
        public QueryModel Model { get; }
        
        public QueryInstance(object query, QueryModel model)
        {
            Object = query;
            Model = model;
        }
    }
}