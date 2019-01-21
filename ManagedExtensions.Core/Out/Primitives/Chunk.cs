using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Out.Primitives
{
    public abstract class Chunk
    {
        public abstract int Width { get; }
        internal abstract int MinWidth { get; }
        internal abstract void Output(int fixedWidth, Align align, Output output);
        internal abstract void Output(Output output);
        public static implicit operator Chunk(string str)
        {
            return new Text(str);
        }
    }
}
