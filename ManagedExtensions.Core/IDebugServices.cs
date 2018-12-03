using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using ManagedExtensions.Core.Out;

namespace ManagedExtensions.Core
{
    public interface IDebugServices
    {
        IDebugClient DebugClient { get; }
        DataTarget DataTarget { get; }
        Output Output { get; }
    }
}
