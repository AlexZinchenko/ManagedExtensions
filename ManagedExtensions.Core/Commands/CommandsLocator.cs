﻿using System;
using System.Collections.Generic;

namespace ManagedExtensions.Core.Commands
{
    public sealed class CommandsLocator : ICommandsLocator
    {
        public TCommand Get<TCommand>() where TCommand : NativeCommand
        {
            if (_commands.ContainsKey(typeof(TCommand)))
                return (TCommand)_commands[typeof(TCommand)];

            throw new InvalidOperationException($"Command {typeof(TCommand)} is not registered");
        }

        public IEnumerable<NativeCommand> AllCommands => _commands.Values;

        public void Register(NativeCommand command)
        {
            var commandType = command.GetType();

            if (!_commands.ContainsKey(commandType))
            {
                _commands.Add(commandType, command);
                return;
            }
            
            throw new InvalidOperationException($"Command of type {commandType} is already registered");
        }
        
        public bool IsRegistered<TCommand>()
        {
            return _commands.ContainsKey(typeof(TCommand));
        }
        
        private Dictionary<Type, NativeCommand> _commands = new Dictionary<Type, NativeCommand>();
    }
}
