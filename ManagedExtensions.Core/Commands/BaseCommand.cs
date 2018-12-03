using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Out;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;

namespace ManagedExtensions.Core.Commands
{
    public abstract class BaseCommand
    {
        public BaseCommand(ICommandsHost host)
        {
            DebugClient = host.DebugServices.DebugClient;
            DataTarget = host.DebugServices.DataTarget;
            Output = host.DebugServices.Output;
            Host = host;

            Runtime = host.Runtime;
            Heap = Runtime.Heap;
            
            Commands = host.Commands;
            ExternalCommandNames = host.ExternalCommandNames;
        }

        public IDebugClient DebugClient { get; private set; }
        public IDebugDataSpaces DebugDataSpaces { get { return (IDebugDataSpaces)DebugClient; } }
        public DataTarget DataTarget { get; private set; }
        public ClrRuntime Runtime { get; private set; }
        public Output Output { get; private set; }
        public ClrHeap Heap { get; private set; }
        public ICommandsLocator Commands { get; private set; }
        public ExternalCommandNameProvider ExternalCommandNames { get; private set; }
        public ICommandsHost Host { get; set; }
        public CommandGroupId Group
        {
            get
            {
                var commandIdAttr = 
                    GetType()
                    .GetCustomAttributes(false)
                    .OfType<CommandGroupAttribute>()
                    .FirstOrDefault();

                return commandIdAttr?.Id ?? CommandGroupId.Common;
            }
        }

        public List<string> ExportedNames
        {
            get
            {
                var commandMethods = GetType()
                    .GetMethods()
                    .Where(m => m.IsStatic && m.GetCustomAttributes(false).OfType<CommandMethodAttribute>().Any())
                    .Select(m => m.Name)
                    .ToList();

                return commandMethods;
            }
        }

        public abstract void WriteHelp();
    }
}
