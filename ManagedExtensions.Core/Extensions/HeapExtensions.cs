using System;
using System.Linq;
using ManagedExtensions.Core.Dynamic;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core.Extensions
{
    public static class HeapExtensions
    {
        public static DynamicInstance GetInstance(this ClrHeap heap, ulong address)
        {
            return new DynamicInstance(address, heap);
        }

        public static ClrType GetType(this ClrHeap heap, string name)
        {
            var type = heap.EnumerateTypes().FirstOrDefault(t => t.Name.EndsWith(name));

            if (type == null)
                throw new Exception($"Type \"{name}\" wasn't found");

            return type;
        }
    }
}
