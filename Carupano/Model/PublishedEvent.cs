namespace Carupano.Model
{
    public class PublishedEvent
    {
        public object Object { get;}
        public long SequenceNo { get; }
        public PublishedEvent(object o, long seqNo)
        {
            Object = o;
            SequenceNo = seqNo;
        }
    }
    public class PersistedEvent
    {
        public object Event { get; }
        public long SequenceNo { get; }
        public PersistedEvent(object evt, long seq)
        {
            Event = evt;
            SequenceNo = seq;
        }
    }
    
}
