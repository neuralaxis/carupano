using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Carupano.Model;
using System.Linq;

namespace Carupano.InMemory
{
    using Persistence;
    public class InMemoryEventStore : IEventStore
    {
        List<Entry> _events = new List<Entry>();
        public IEnumerable Load(string aggregate, string id)
        {
            return _events.Where(c => c.Aggregate == aggregate && c.Id == id).SelectMany(c => c.Events).Select(c => c.Event);
        }

        public IEnumerable<PersistedEvent> Load(long seqNum)
        {
            return _events.SelectMany(c => c.Events).Where(c => c.SequenceNo >= seqNum);
        }

        public IEnumerable<PersistedEvent> Save(string aggregate, string id, IEnumerable events)
        {
            long seqNum = _events.Any() ? _events.SelectMany(c => c.Events).Max(c => c.SequenceNo) : -1;
            var entry = new Entry(seqNum, aggregate, id);
            foreach (var evt in events)
            {
                seqNum += 1;
                entry.Events.Add(new PersistedEvent(evt, seqNum));
            }
            _events.Add(entry);
            return entry.Events;
        }

        class Entry
        {
            public long SequenceNumber { get; }
            public string Aggregate { get; }
            public string Id { get; }
            public List<PersistedEvent> Events { get; }
            public Entry(long seqNum, string aggregate, string id)
            {
                SequenceNumber = seqNum;
                Aggregate = aggregate;
                Id = id;
                Events = new List<PersistedEvent>();
            }
        }
    }

}
