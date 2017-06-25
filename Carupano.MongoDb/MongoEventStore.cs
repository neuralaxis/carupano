using Carupano.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Carupano.Model;
using System.Collections;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Linq;

namespace Carupano.MongoDb
{
    public class MongoEventStore : IEventStore
    {
        IMongoCollection<EventEntry> _events;
        FilterDefinitionBuilder<EventEntry> Filter;
        UpdateDefinitionBuilder<EventEntry> Update;
        public MongoEventStore(string url) :
            this(new MongoUrl(url))
        { }
        public MongoEventStore(MongoUrl url)
        {
            var mongo = new MongoClient(url);
            var db = mongo.GetDatabase(url.DatabaseName);
            BsonClassMap.RegisterClassMap<EventEntry>(cfg => {
                cfg.MapIdField(c => c.Id);
                cfg.AutoMap();
            });
            _events = db.GetCollection<EventEntry>("events");
            Filter = Builders<EventEntry>.Filter;
            Update = Builders<EventEntry>.Update;
        }
        public IEnumerable<PersistedEvent> Load(string aggregate, string id)
        {
            return _events.Find(Filter.And(Filter.Eq(c => c.Aggregate, aggregate), Filter.Eq(c => c.Id, id))).ToList().Select(c => new PersistedEvent(c.Event, c.SequenceNo));
        }

        public IEnumerable<PersistedEvent> Load(long seqNum)
        {
            return _events.Find(Filter.Gte(c => c.SequenceNo, seqNum)).ToList().Select(c => new PersistedEvent(c.Event, c.SequenceNo));
        }

        public IEnumerable<PersistedEvent> Save(string aggregate, string id, IEnumerable events)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _events.DeleteMany(Filter.Empty);
        }
    }

    public class EventEntry
    {
        public string Id { get; set; }
        public long SequenceNo { get; set; }
        public string Aggregate { get; set; }
        public string AggregateId { get; set; }
        public object Event { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
