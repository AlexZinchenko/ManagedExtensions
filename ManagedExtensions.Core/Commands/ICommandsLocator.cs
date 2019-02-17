using System.Collections.Generic;

namespace ManagedExtensions.Core.Commands
{
    public interface ICommandsLocator
    {
        IEnumerable<NativeCommand> AllCommands { get; }

        TCommand Get<TCommand>() where TCommand : NativeCommand;
        bool IsRegistered<TCommand>();

        void Register(NativeCommand command);
    }
}
