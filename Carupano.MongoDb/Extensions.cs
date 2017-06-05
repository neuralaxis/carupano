using System;
using System.Collections.Generic;
using System.Text;

namespace Carupano.Configuration
{
    using global::MongoDB.Driver;
    using MongoDB;
    public static class Extensions
    {
        public static ProjectionModelBuilder<T> WithMongoState<T>(this ProjectionModelBuilder<T> builder, string connectionString)
        {
            return builder.WithState(new MongoProjectionStateProvider(MongoUrl.Create(connectionString)));
        }
    }
}
