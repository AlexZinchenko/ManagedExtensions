using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Commands
{
    public interface INativeCommandsHost
    {
        IDebugServices DebugServices { get; }
        ICommandsLocator Commands { get; }
        ExternalCommandNameProvider ExternalCommandNames { get; }
        bool OnlyNativeCommandsAreAvailable { get; }
        NativeCommand FindCommandByMethodName(string methodName);
    }
}
