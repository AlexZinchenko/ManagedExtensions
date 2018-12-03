using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.Runtime.InteropServices;
using ManagedExtensions.Core.Out;
using RuntimeDataTarget = Microsoft.Diagnostics.Runtime.DataTarget;

namespace ManagedExtensions.Core
{
    public sealed class DebugServices : IDebugServices
    {
        public DebugServices(IntPtr ptrClient)
        {
            DebugClient = (IDebugClient)Marshal.GetUniqueObjectForIUnknown(ptrClient);
            DataTarget = RuntimeDataTarget.CreateFromDebuggerInterface(DebugClient);
            Output = new Output(DebugClient);
        }

        public IDebugClient DebugClient { get; private set; }
        public DataTarget DataTarget { get; private set; }
        public Output Output { get; private set; }
    }
}
