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

        public override string ToString()
        {
            return string.Format(_linkTemplate, Cmd, DisplayName);
        }

        internal override void Output(int fixedWidth, Align align, Output output)
        {
            if (fixedWidth < Width)
            {
                throw new ArgumentException($"Can't output link in {fixedWidth} width, width = {Width}");
            }

            output.Write("{0}", new Link(DisplayName.InFixedSpace(fixedWidth, align), Cmd));
        }

        internal override void Output(Output output)
        {
            output.Write("{0}", this);
        }

        private readonly string _linkTemplate = "<link cmd=\"{0}\">{1}</link>";
    }
}
