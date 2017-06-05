using Carupano.Model;

namespace Carupano
{
    interface IAggregateManager
    {
        CommandExecutionResult ExecuteCommand(object command);
    }
}
