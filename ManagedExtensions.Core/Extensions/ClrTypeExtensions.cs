using System.Collections.Generic;
using System.Linq;
using ManagedExtensions.Core.Dynamic;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Extensions
{
    public static class ClrTypeExtensions
    {
        public static IEnumerable<DynamicInstance> GetObjects(this ClrType type)
        {
            var heap = type.Heap;

            return heap
                .EnumerateObjectAddresses()
                .Where(address => heap.GetObjectType(address) == type)
                .Select(address => heap.GetInstance(address));
        }
    }
}
