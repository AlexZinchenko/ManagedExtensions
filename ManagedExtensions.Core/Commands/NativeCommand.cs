using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Out;
using ManagedExtensions.Core.Out.Primitives;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;

namespace ManagedExtensions.Core.Commands
{
    public abstract class NativeCommand
    {
        public NativeCommand(INativeCommandsHost host)
        {
            DebugClient = host.DebugServices.DebugClient;
            DataTarget = host.DebugServices.DataTarget;
            Output = host.DebugServices.Output;
            Host = host;

            Commands = host.Commands;
            ExternalCommandNames = host.ExternalCommandNames;

            ChunkFactory = new ChunkFactory();
        }

        public IDebugClient DebugClient { get; private set; }
        public IDebugDataSpaces DebugDataSpaces { get { return (IDebugDataSpaces)DebugClient; } }
        public DataTarget DataTarget { get; private set; }
        public Output Output { get; private set; }
        public ChunkFactory ChunkFactory { get; private set; }
        public ICommandsLocator Commands { get; private set; }
        public ExternalCommandNameProvider ExternalCommandNames { get; private set; }
        public INativeCommandsHost Host { get; set; }
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
                    .Select(m => AdjustName(m.Name))
                    .ToList();

                return commandMethods;
            }
        }

        public abstract void WriteHelp();

        private string AdjustName(string name)
        {
            if (name.StartsWith("_") && name.EndsWith("_"))
                name = name.Substring(1, name.Length - 2);

            return name;
        }
    }
}
