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
    
}
