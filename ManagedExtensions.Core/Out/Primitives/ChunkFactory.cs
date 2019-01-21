using ManagedExtensions.Core.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Out.Primitives
{
    public sealed class ChunkFactory
    {
        public Chunk CreateTextOrLink(DynamicInstance instance, Func<string> dumpCmd)
        {
            var displayedValue = instance.DisplayedPrimitiveValue;
            if (displayedValue != null)
            {
                if (instance.IsString)
                {
                    displayedValue = $"\"{displayedValue}\"";
                }
                return new Text(displayedValue);
            }

            return new Link(instance.HexAddress, dumpCmd());
        }
    }
}
