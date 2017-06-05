namespace Carupano.Model
{
    public class CommandInstance
    {
        public CommandModel Model { get; private set; }
        public object Instance { get; private set; }
        public string AggregateId { get { return Model.Correlation.GetAggregateId(Instance); } }

        public CommandInstance(CommandModel model, object instance)
        {
            Model = model;
            Instance = instance;
        }
    }
    
}
