namespace Carupano
{
    public interface IQuery<TQueryType,TResponseType>
    {
        TResponseType Query(TQueryType query);
    }
}
