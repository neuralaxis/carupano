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
    }
    public class QueryInstance
    {
        public object Query { get; }
        public QueryModel Model { get; }
        public QueryInstance(object query, QueryModel model)
        {
            Query = query;
        }
    }
}