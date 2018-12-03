using System.Collections.Generic;

namespace ManagedExtensions.Core.Commands
{
    public interface ICommandsLocator
    {
        IEnumerable<BaseCommand> AllCommands { get; }

        TCommand Get<TCommand>() where TCommand : BaseCommand;
        bool IsRegistered<TCommand>();

        void Register(BaseCommand command);
    }
}
