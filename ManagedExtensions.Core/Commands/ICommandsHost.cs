using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Commands
{
    public interface ICommandsHost
    {
        IDebugServices DebugServices { get; }
        ClrRuntime Runtime { get; }
        ICommandsLocator Commands { get; }
        ExternalCommandNameProvider ExternalCommandNames { get; }

        BaseCommand FindCommandByMethodName(string methodName);
    }
}
