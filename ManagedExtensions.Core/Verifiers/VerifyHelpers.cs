using ManagedExtensions.Core.Out;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Verifiers
{
    public static class VerifyHelpers
    {
        public static bool VerifyObjAddress(ulong addr, ClrHeap heap, Output output)
        {
            var type = heap.GetObjectType(addr);

            if (type == null)
                output.WriteErrorLine($"{addr:x} isn't a valid reference object address");

            return type != null;
        }
    }
}
