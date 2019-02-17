using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Commands
{
    public interface IManagedCommandsHost : INativeCommandsHost
    {
        ClrRuntime ClrRuntime { get; }
    }
}
