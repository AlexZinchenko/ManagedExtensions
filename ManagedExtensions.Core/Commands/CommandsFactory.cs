using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Commands
{
    public sealed class CommandsFactory// : ICommandsFactory
    {
        public CommandsFactory(ICommandsHost commandsHost)
        {
            _commandsHost = commandsHost;
        }

        //public TCommand CreateCommand<TCommand>() where TCommand : BaseCommand
        //{
        //    return (TCommand)CreateCommand(typeof(TCommand));
        //}

        public BaseCommand CreateCommand(Type commandType)
        {
            if (!typeof(BaseCommand).IsAssignableFrom(commandType))
                throw new ArgumentException($"{commandType} must be inherited from BaseCommand", nameof(commandType));

            return (BaseCommand)Activator.CreateInstance(commandType, _commandsHost);
        }

        private ICommandsHost _commandsHost;
    }
}
