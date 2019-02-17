using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Exceptions;
using ManagedExtensions.Core.Out;

namespace ManagedExtensions.Core.Commands
{
    internal sealed class CommandsHost : IManagedCommandsHost
    {
        internal CommandsHost(IDebugServices debugServices) : this(debugServices, null)
        {
        }

        internal CommandsHost(IDebugServices debugServices, ClrRuntime runtime)
        {
            DebugServices = debugServices;
            ClrRuntime = runtime;

            ExternalCommandNames = new ExternalCommandNameProvider();
            Commands = new CommandsLocator();

            _commandsFactory = new CommandsFactory(this);
            AddCommands();
        }

        public IDebugServices DebugServices { get; private set; }
        public ClrRuntime ClrRuntime { get; private set; }
        public bool IsNativeDump => ClrRuntime == null;
        public bool OnlyNativeCommandsAreAvailable => IsNativeDump;
        public ICommandsLocator Commands { get; private set; }
        public ExternalCommandNameProvider ExternalCommandNames { get; private set; }
        public Output Output => DebugServices.Output;

        public void Execute<TCommand>(Action<TCommand> commandMethod) where TCommand : NativeCommand
        {
            var command = Commands.Get<TCommand>();

            try
            {
                commandMethod(command);
            }
            catch (CommandException e)
            {
                Output.WriteErrorLine(e.Message);
            }
        }

        public NativeCommand FindCommandByMethodName(string methodName)
        {
            if (_methodMap.ContainsKey(methodName))
                return _methodMap[methodName];

            return null;
        }

        private void AddCommands()
        {
            var allTypes = GetType()
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(NativeCommand).IsAssignableFrom(t));

            if (IsNativeDump)
            {
                allTypes = allTypes.Where(t => !typeof(ManagedCommand).IsAssignableFrom(t));
            }

            foreach (var commandType in allTypes)
            {
                var command = _commandsFactory.CreateCommand(commandType);

                var exportedNames = command.ExportedNames;
                if (command.ExportedNames.Any())
                {
                    Commands.Register(command);

                    foreach (var exportedName in exportedNames)
                    {
                        _methodMap.Add(exportedName, command);
                    }
                }
                else
                {
                    Output.WriteWarningLine($"command of type {command.GetType()} isn't registered: it hasn't exported methods");
                }
            }
        }

        private CommandsFactory _commandsFactory;
        private Dictionary<string, NativeCommand> _methodMap = new Dictionary<string, NativeCommand>();
    }
}
