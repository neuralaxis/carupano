using Carupano.Model;

namespace Carupano
{
    public interface IAggregateManager
    {
        CommandExecutionResult ExecuteCommand(object command);
    }
}
