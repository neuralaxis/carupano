using System;

namespace Carupano.MongoDB
{
    using global::MongoDB.Driver;
    using Model;
    public class MongoProjectionStateProvider : IProjectionStateProvider
    {
        IMongoCollection<ProjectionState> Collection;
        FilterDefinitionBuilder<ProjectionState> Filters;
        UpdateDefinitionBuilder<ProjectionState> Update;
        public MongoProjectionStateProvider(MongoUrl url)
        {
            var mongo = new MongoClient(url);
            var db = mongo.GetDatabase(url.DatabaseName);
            Collection = db.GetCollection<ProjectionState>("projectionstate");
            Filters = Builders<ProjectionState>.Filter;
            Update = Builders<ProjectionState>.Update;
        }

        FilterDefinition<ProjectionState> QueryFor(object instance)
        {
            var id = instance.GetType().Name;
            return Filters.Eq(c=>c.Id, id);
        }
        public long Get(object instance)
        {
            var single = Collection.Find(QueryFor(instance)).SingleOrDefault();
            if (single != null) return single.SeqNum;
            return default(long);
        }

        public void Set(object instance, long seqNum)
        {
            var opts = new UpdateOptions();
            opts.IsUpsert = true;
            Collection.UpdateOne(QueryFor(instance), Update.Set(c => c.SeqNum, seqNum), opts);
        }

        class ProjectionState
        {
            public string Id { get; set; }
            public long SeqNum { get; set; }
        }
    }
}
