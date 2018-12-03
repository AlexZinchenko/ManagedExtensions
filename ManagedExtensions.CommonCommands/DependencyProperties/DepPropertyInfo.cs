using ManagedExtensions.Core.Dynamic;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.CommonCommands.DependencyProperties
{
    public sealed class DepPropertyInfo
    {
        public DepPropertyInfo(ClrType propertyType, DynamicInstance propertyValue, string propertyName)
        {
            Type = propertyType;
            Value = propertyValue;
            Name = propertyName;
        }

        public ClrType Type { get; private set; }
        public DynamicInstance Value { get; private set; }
        public string Name { get; private set; }
    }
}
