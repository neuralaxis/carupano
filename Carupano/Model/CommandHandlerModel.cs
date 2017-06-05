using System.Linq;
using System.Reflection;

namespace Carupano.Model
{
    public class CommandHandlerModel
    {
        public MethodInfo Method { get; private set; }
        public CommandModel Command { get; private set; }

        public CommandHandlerModel(MethodInfo info, CommandModel model)
        {
            Method = info;
            Command = model;
        }
        public CommandHandlerModel(MethodInfo method)
        {
            Method = method;
            Command = new CommandModel(method.GetParameters().First().ParameterType);
        }
        public bool Handles(CommandModel model)
        {
            return model.TargetType == Command.TargetType;
        }
    }
    
}
