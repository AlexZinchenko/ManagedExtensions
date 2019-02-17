using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Commands
{
    internal sealed class CommandsFactory
    {
        public CommandsFactory(INativeCommandsHost commandsHost)
        {
            _commandsHost = commandsHost;
        }
        
        public NativeCommand CreateCommand(Type commandType)
        {
            if (!typeof(NativeCommand).IsAssignableFrom(commandType))
                throw new ArgumentException($"{commandType} must be inherited from {typeof(NativeCommand)}", nameof(commandType));

            return (NativeCommand)Activator.CreateInstance(commandType, _commandsHost);
        }

        private INativeCommandsHost _commandsHost;
    }
}
