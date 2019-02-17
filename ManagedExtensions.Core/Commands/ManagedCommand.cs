using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Commands
{
    public abstract class ManagedCommand : NativeCommand
    {
        public ManagedCommand(IManagedCommandsHost host) : base(host)
        {
            Runtime = host.ClrRuntime;
            Heap = Runtime.Heap;
        }

        public ClrRuntime Runtime { get; private set; }
        public ClrHeap Heap { get; private set; }
    }
}
