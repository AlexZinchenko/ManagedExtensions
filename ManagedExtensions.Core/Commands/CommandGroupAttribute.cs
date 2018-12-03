using System;

namespace ManagedExtensions.Core.Commands
{
    public class CommandGroupAttribute : Attribute
    {
        public CommandGroupAttribute()
        {
            Id = CommandGroupId.Common;
        }

        public CommandGroupId Id { get; set; }
    }
}
