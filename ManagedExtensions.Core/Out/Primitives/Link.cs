using ManagedExtensions.Core.Extensions;
using ManagedExtensions.Core.Out.Primitives;
using System;

namespace ManagedExtensions.Core.Out
{
    public sealed class Link : Chunk
    {
        public Link(object displayName, string cmd)
        {
            Cmd = cmd;
            DisplayName = displayName.ToString();
        }

        public string Cmd { get; private set; }
        public string DisplayName { get; private set; }

        public override int Width => DisplayName.Length;

        internal override int MinWidth => Width;

        internal override void Output(int fixedWidth, Align align, Output output)
        {
            if (fixedWidth < Width)
            {
                throw new ArgumentException($"Can't output link in {fixedWidth} width, width = {Width}");
            }

            Write(DisplayName.InFixedSpace(fixedWidth, align), Cmd, output);
        }

        public override void Output(Output output)
        {
            Write(DisplayName, Cmd, output);
        }

        private void Write(string displayName, string cmd, Output output)
        {
            output.WriteDml("<link cmd=\"{0}\">{1}</link>", cmd, displayName);
        }
    }
}
