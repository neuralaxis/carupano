using Carupano.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano
{
    public interface IEventStore
    {
        IEnumerable Load(string aggregate, string id);
        IEnumerable<PersistedEvent> Save(string aggregate, string id, IEnumerable @events);
        IEnumerable<PersistedEvent> Load(long seqNum);
    }
}
