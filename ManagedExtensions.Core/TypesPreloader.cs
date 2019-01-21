using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core
{
    internal class TypesPreloader
    {
        public void LoadAllTypes(IDebugServices debugServices, ClrRuntime runtime)
        {
            var sw = new Stopwatch();
            sw.Start();

            var heap = runtime.Heap;
            foreach (var objAddress in heap.EnumerateObjectAddresses())
            {
                heap.GetObjectType(objAddress);
            }

            sw.Stop();
            debugServices.Output.WriteLine($"Types preload was finished in {sw.ElapsedMilliseconds} ms");
        }
    }
}
